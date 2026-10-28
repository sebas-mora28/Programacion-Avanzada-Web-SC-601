"""Pruebas HTTP de la solución compilada, usando solo la biblioteca estándar."""

import base64
from contextlib import contextmanager
import json
import os
from pathlib import Path
import subprocess
import tempfile
import time
import urllib.error
import urllib.request


ROOT = Path(__file__).resolve().parent
CLIENT = urllib.request.build_opener(urllib.request.ProxyHandler({}))
CHECKS = 0


def check(condition, message):
    global CHECKS
    if not condition:
        raise AssertionError(message)
    CHECKS += 1


def request(base, method, path, expected, body=None, authorization=None):
    headers = {}
    if authorization is not None:
        headers['Authorization'] = authorization
    data = None
    if body is not None:
        data = json.dumps(body).encode()
        headers['Content-Type'] = 'application/json'
    req = urllib.request.Request(base + path, data=data, headers=headers, method=method)
    try:
        response = CLIENT.open(req, timeout=5)
    except urllib.error.HTTPError as error:
        response = error
    with response:
        raw = response.read()
        check(response.status == expected,
              f'{method} {path}: esperado {expected}, recibido {response.status}: {raw[:300]!r}')
        value = json.loads(raw) if raw and 'application/json' in response.headers.get('Content-Type', '') else raw
        return response.headers, value


@contextmanager
def server(project):
    dll = ROOT / 'src' / project / 'bin' / 'Debug' / 'net10.0' / f'{project}.dll'
    if not dll.exists():
        raise RuntimeError('Primero ejecute dotnet build CP1-resuelto.slnx.')
    env = os.environ.copy()
    env['ASPNETCORE_ENVIRONMENT'] = 'Development'
    env['Logging__LogLevel__Default'] = 'Information'
    env.pop('ASPNETCORE_HTTPS_PORT', None)
    with tempfile.TemporaryFile(mode='w+') as log:
        process = subprocess.Popen(
            ['dotnet', str(dll), '--urls', 'http://127.0.0.1:0'],
            cwd=dll.parents[3], env=env, stdout=log, stderr=log)
        try:
            deadline = time.monotonic() + 20
            base = None
            while time.monotonic() < deadline:
                log.seek(0)
                output = log.read()
                for line in output.splitlines():
                    if 'Now listening on: http://' in line:
                        base = line.split('Now listening on: ', 1)[1].strip()
                        break
                if base:
                    break
                if process.poll() is not None:
                    raise RuntimeError(f'{project} no inició:\n{output}')
                time.sleep(0.1)
            if not base:
                raise RuntimeError(f'Tiempo agotado al iniciar {project}:\n{output}')
            yield base
        finally:
            if process.poll() is None:
                process.terminate()
                try:
                    process.wait(timeout=10)
                except subprocess.TimeoutExpired:
                    process.kill()
                    process.wait()


def basic(credentials, scheme='Basic'):
    return scheme + ' ' + base64.b64encode(credentials.encode()).decode()


def test_basic(base):
    request(base, 'GET', '/', 200)
    invalid = [None, 'Basic !!!', 'Bearer token', 'Basic', basic('admin'),
               basic(':passwordAdmin'), basic('admin:'), basic('admin:mal'),
               basic('admin:passwordAdmin:extra'), 'Basic /w==']
    for authorization in invalid:
        headers, _ = request(base, 'GET', '/api/basic/profile', 401, authorization=authorization)
        check(headers.get('WWW-Authenticate') == 'Basic realm="SC701", charset="UTF-8"', 'Challenge Basic')
    for username, password, user_id, role in [
        ('admin', 'passwordAdmin', '1', 'Admin'),
        ('estudiante', 'password123', '2', 'User'),
    ]:
        _, profile = request(base, 'GET', '/api/basic/profile', 200,
                             authorization=basic(f'{username}:{password}', 'bAsIc'))
        check(profile == {'id': user_id, 'username': username, 'role': role}, 'Perfil sin contraseñas')
    request(base, 'GET', '/api/basic/admin', 401)
    request(base, 'GET', '/api/basic/admin', 401, authorization=basic('admin:mal'))
    request(base, 'GET', '/api/basic/admin', 200, authorization=basic('admin:passwordAdmin'))
    request(base, 'GET', '/api/basic/admin', 403, authorization=basic('estudiante:password123'))


def test_products(base):
    _, initial = request(base, 'GET', '/products', 200)
    check(len(initial) == 3, 'Catálogo inicial')
    request(base, 'GET', '/products/999', 404)
    headers, product = request(base, 'POST', '/products', 201, {'name': 'Webcam', 'price': 65})
    location = headers.get('Location')
    check(location == f'/products/{product["id"]}', 'Location del producto')
    _, found = request(base, 'GET', location, 200)
    check(found == product, 'Consultar producto creado')
    for body in [{'name': '', 'price': 1}, {'name': '   ', 'price': 1},
                 {'name': None, 'price': 1}, {'price': 1}, {'name': 'X', 'price': -1}]:
        request(base, 'POST', '/products', 400, body)
        request(base, 'PUT', location, 400, body)
    _, unchanged = request(base, 'GET', location, 200)
    check(unchanged == product, 'PUT inválido no modifica el producto')
    _, catalog = request(base, 'GET', '/products', 200)
    check(len(catalog) == 4, 'POST inválido no crea productos')
    update = {'name': 'Nueva webcam', 'price': 10}
    _, first = request(base, 'PUT', location, 200, update)
    _, second = request(base, 'PUT', location, 200, update)
    check(first == second and second['price'] == 10, 'PUT idempotente')
    request(base, 'PUT', '/products/999', 404, update)
    request(base, 'PUT', location, 200, {'name': 'Gratis', 'price': 0})
    _, empty = request(base, 'DELETE', location, 204)
    check(empty == b'', 'DELETE sin cuerpo')
    request(base, 'DELETE', location, 404)
    request(base, 'GET', location, 404)
    request(base, 'GET', '/products/create?name=X&price=1', 404)
    request(base, 'GET', '/products/1/delete', 404)
    request(base, 'POST', '/products/1/update', 404, update)


def test_tasks(base):
    for body in [{}, {'seconds': 0}, {'seconds': -1}, {'seconds': 61},
                 {'seconds': 1.5}, {'seconds': 'uno'}, {'seconds': None}]:
        request(base, 'POST', '/tasks', 400, body)
    request(base, 'GET', '/tasks/ffffffff-ffff-ffff-ffff-ffffffffffff', 404)
    started = time.monotonic()
    headers, task = request(base, 'POST', '/tasks', 202, {'seconds': 2})
    check(time.monotonic() - started < 2, 'POST devuelve antes de terminar el procesamiento')
    check(task['status'] == 'Pending', 'Estado inicial Pending')
    location = headers.get('Location')
    check(location == f'/tasks/{task["id"]}', 'Location de la tarea')
    _, queued = request(base, 'POST', '/tasks', 202, {'seconds': 1})
    _, waiting = request(base, 'GET', f'/tasks/{queued["id"]}', 200)
    check(waiting['status'] == 'Pending', 'Segunda tarea espera en la cola')
    states = set()
    deadline = time.monotonic() + 10
    while time.monotonic() < deadline:
        _, current = request(base, 'GET', location, 200)
        check(current['id'] == task['id'], 'Identificador estable')
        states.add(current['status'])
        if current['status'] in ('Completed', 'Failed'):
            break
        time.sleep(0.2)
    check('Processing' in states, 'La tarea pasa por Processing')
    check(bool(states & {'Completed', 'Failed'}), 'La tarea alcanza un estado final')
    request(base, 'POST', '/tasks', 202, {'seconds': 60})


if __name__ == '__main__':
    for name, test in [('BasicApp', test_basic), ('ErroresHTTP', test_products), ('TareasHTTP', test_tasks)]:
        with server(name) as url:
            test(url)
        print(f'{name}: OK')
    print(f'{CHECKS} comprobaciones correctas.')
