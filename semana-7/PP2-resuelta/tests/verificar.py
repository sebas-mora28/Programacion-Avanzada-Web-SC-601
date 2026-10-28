#!/usr/bin/env python3
"""Pruebas HTTPS reales. Requiere dotnet build previo, Python 3 y OpenSSL."""
import base64
import hashlib
import hmac
import http.cookiejar
import json
import os
from pathlib import Path
import secrets
import ssl
import subprocess
import tempfile
import time
import urllib.error
import urllib.parse
import urllib.request
from html.parser import HTMLParser

ROOT = Path(__file__).resolve().parents[1]
USERS = [("admin", "Admin123!", "1", "Admin"), ("estudiante", "User123!", "2", "User")]
RESULTS = []


class NoRedirect(urllib.request.HTTPRedirectHandler):
    def redirect_request(self, req, fp, code, msg, headers, newurl):
        return None


class FormParser(HTMLParser):
    token = None

    def handle_starttag(self, tag, attrs):
        attrs = dict(attrs)
        if tag == "input" and attrs.get("name") == "__RequestVerificationToken":
            self.token = attrs["value"]


def client():
    # Solo se omite la confianza del certificado efímero de estas pruebas locales.
    return urllib.request.build_opener(
        NoRedirect(), urllib.request.HTTPSHandler(context=ssl._create_unverified_context()),
        urllib.request.HTTPCookieProcessor(http.cookiejar.CookieJar()))


def request(opener, port, path, data=None, headers=None):
    req = urllib.request.Request(f"https://localhost:{port}{path}", data=data, headers=headers or {})
    try:
        response = opener.open(req, timeout=10)
    except urllib.error.HTTPError as error:
        response = error
    with response:
        return response.status, response.headers, response.read().decode()


def check(label, response, status, predicate=lambda headers, body: True):
    actual, headers, body = response
    assert actual == status and predicate(headers, body), f"{label}: HTTP {actual}; esperado {status} o contenido incorrecto"
    RESULTS.append(f"PASS | {label} | HTTP {actual}")
    print(RESULTS[-1], flush=True)
    return response


def form(opener, path):
    response = request(opener, 17102, path)
    assert response[0] == 200
    parser = FormParser()
    parser.feed(response[2])
    assert parser.token, "Falta el token antiforgery en el formulario"
    return parser.token


def post_form(opener, path, fields):
    return request(opener, 17102, path, urllib.parse.urlencode(fields).encode(),
                   {"Content-Type": "application/x-www-form-urlencoded"})


def b64(value):
    return base64.urlsafe_b64encode(value).decode().rstrip("=")


def main():
    processes = []
    with tempfile.TemporaryDirectory(prefix="pp2-pruebas-") as directory:
        temp = Path(directory)
        key = secrets.token_urlsafe(48)
        cert_password = secrets.token_urlsafe(24)
        env = os.environ.copy()
        env["PP2_CERT_PASSWORD"] = cert_password
        subprocess.run(["openssl", "req", "-x509", "-newkey", "rsa:2048", "-nodes",
                        "-keyout", str(temp / "key.pem"), "-out", str(temp / "cert.pem"),
                        "-days", "1", "-subj", "/CN=localhost"], check=True, capture_output=True)
        subprocess.run(["openssl", "pkcs12", "-export", "-out", str(temp / "cert.pfx"),
                        "-inkey", str(temp / "key.pem"), "-in", str(temp / "cert.pem"),
                        "-passout", "env:PP2_CERT_PASSWORD"], env=env, check=True, capture_output=True)
        env.update({"ASPNETCORE_ENVIRONMENT": "Development",
                    "ASPNETCORE_Kestrel__Certificates__Default__Path": str(temp / "cert.pfx"),
                    "ASPNETCORE_Kestrel__Certificates__Default__Password": cert_password,
                    "Jwt__SigningKey": key, "Jwt__ExpirationMinutes": "1"})
        logs = []
        try:
            for app, port in [("BasicApp", 17101), ("CookiesApp", 17102), ("JwtApp", 17103)]:
                project = ROOT / "src" / app
                log = (temp / f"{app}.log").open("w+")
                logs.append(log)
                app_env = dict(env, ASPNETCORE_URLS=f"https://localhost:{port}")
                processes.append(subprocess.Popen(
                    ["dotnet", str(project / "bin/Debug/net10.0" / f"{app}.dll")],
                    cwd=project, env=app_env, stdout=log, stderr=subprocess.STDOUT))
            basic, jwt = client(), client()
            for port, process in zip([17101, 17102, 17103], processes):
                for _ in range(100):
                    if process.poll() is not None:
                        raise RuntimeError(f"La aplicación del puerto {port} terminó al iniciar")
                    try:
                        request(client(), port, "/")
                        break
                    except urllib.error.URLError:
                        time.sleep(0.1)
                else:
                    raise RuntimeError(f"La aplicación del puerto {port} no inició")

            def issue(username, password):
                return request(jwt, 17103, "/api/auth/token", json.dumps(
                    {"username": username, "password": password}).encode(), {"Content-Type": "application/json"})

            def profile(token):
                return request(jwt, 17103, "/api/jwt/profile", headers={"Authorization": f"Bearer {token}"})

            first = check("JWT emisión admin", issue("admin", "Admin123!"), 200,
                          lambda h, b: json.loads(b)["tokenType"] == "Bearer" and json.loads(b)["expiresIn"] == 60)
            token = json.loads(first[2])["accessToken"]
            payload = json.loads(base64.urlsafe_b64decode(token.split(".")[1] + "=="))
            check("JWT antes de expirar", profile(token), 200)
            assert not any("password" in name.lower() for name in payload)

            challenge = lambda h, b: h.get("WWW-Authenticate") == 'Basic realm="SC701", charset="UTF-8"'
            for path in ["/api/basic/profile", "/api/basic/admin"]:
                check(f"Basic sin credenciales {path}", request(basic, 17101, path), 401, challenge)
            for username, password, uid, role in USERS:
                auth = {"Authorization": "bAsIc " + base64.b64encode(f"{username}:{password}".encode()).decode()}
                check(f"Basic perfil {username}", request(basic, 17101, "/api/basic/profile", headers=auth), 200,
                      lambda h, b: json.loads(b) == {"id": uid, "username": username, "role": role})
                check(f"Basic administrador {username}", request(basic, 17101, "/api/basic/admin", headers=auth),
                      200 if role == "Admin" else 403)
            for label, value in [("Base64 inválido", "Basic !!!"), ("sin separador", "Basic YWRtaW4="),
                                 ("campos vacíos", "Basic Og=="), ("sin contraseña", "Basic YWRtaW46"),
                                 ("sin usuario", "Basic OkFkbWluMTIzIQ=="), ("sin parámetro", "Basic"),
                                 ("esquema incorrecto", "Bearer invalido"), ("UTF-8 inválido", "Basic /zo="),
                                 ("contraseña incorrecta", "Basic YWRtaW46bWFs"),
                                 ("contraseña con dos puntos", "Basic " + base64.b64encode(b"admin:Admin123!:extra").decode())]:
                for path in ["/api/basic/profile", "/api/basic/admin"]:
                    check(f"Basic {label} {path}", request(basic, 17101, path, headers={"Authorization": value}), 401, challenge)

            redirect_login = lambda h, b: urllib.parse.urlparse(h.get("Location", "")).path == "/Login"
            for username, password, uid, role in USERS:
                cookies = client()
                check(f"Cookies sin sesión {username}", request(cookies, 17102, "/Dashboard"), 302, redirect_login)
                check("Login sin antiforgery", post_form(cookies, "/Login", {"Username": username, "Password": password}), 400)
                csrf = form(cookies, "/Login")
                check(f"Login incorrecto {username}", post_form(cookies, "/Login", {
                    "Username": username, "Password": "incorrecta", "__RequestVerificationToken": csrf}), 200,
                    lambda h, b: "Usuario o contrase" in b and "SC701.Cookies=" not in str(h))
                check("Login incorrecto no abre sesión", request(cookies, 17102, "/Dashboard"), 302, redirect_login)
                csrf = form(cookies, "/Login")
                target = "/Dashboard?origen=prueba" if username == "admin" else "https://example.org/externo"
                expected = target if username == "admin" else "/Dashboard"
                response = check(f"Login válido y retorno seguro {username}", post_form(cookies, "/Login", {
                    "Username": username, "Password": password, "ReturnUrl": target, "__RequestVerificationToken": csrf}),
                    302, lambda h, b: h.get("Location") == expected)
                cookie = next(c for c in response[1].get_all("Set-Cookie", []) if c.startswith("SC701.Cookies="))
                assert all(flag in cookie.lower() for flag in ["httponly", "secure", "samesite=lax"])
                RESULTS.append(f"PASS | Cookie {username} | HttpOnly; Secure; SameSite=Lax")
                print(RESULTS[-1], flush=True)
                check(f"Dashboard {username}", request(cookies, 17102, "/Dashboard"), 200,
                      lambda h, b: username in b and f"<dd>{role}</dd>" in b and f"<dd>{uid}</dd>" in b and h.get("Cache-Control") == "no-store")
                check("GET Logout conserva sesión", request(cookies, 17102, "/Logout"), 200)
                check("Dashboard después de GET Logout", request(cookies, 17102, "/Dashboard"), 200)
                check("Logout sin antiforgery", post_form(cookies, "/Logout", {}), 400)
                check("Logout inválido conserva sesión", request(cookies, 17102, "/Dashboard"), 200)
                csrf = form(cookies, "/Dashboard")
                check(f"POST Logout {username}", post_form(cookies, "/Logout", {"__RequestVerificationToken": csrf}), 302, redirect_login)
                check(f"Dashboard después de logout {username}", request(cookies, 17102, "/Dashboard"), 302, redirect_login)

            for username, password, uid, role in USERS:
                emitted = check(f"JWT emisión {username}", issue(username, password), 200)
                check(f"JWT perfil {username}", profile(json.loads(emitted[2])["accessToken"]), 200,
                      lambda h, b: json.loads(b) == {"id": uid, "username": username, "role": role})
            check("JWT credenciales inválidas", issue("admin", "incorrecta"), 401)
            check("JWT datos incompletos", request(jwt, 17103, "/api/auth/token", b'{"username":"admin"}',
                  {"Content-Type": "application/json"}), 400)
            check("JWT campos vacíos", issue("", ""), 400)
            check("JWT ausente", request(jwt, 17103, "/api/jwt/profile"), 401)
            check("JWT mal formado", profile("esto-no-es-un-jwt"), 401)
            header, _, signature = token.split(".")
            altered = dict(payload, role="OtroRol")
            check("JWT payload alterado conservando firma", profile(f"{header}.{b64(json.dumps(altered).encode())}.{signature}"), 401)

            def signed(claims, algorithm="HS256", signing_key=key):
                content = b64(json.dumps({"alg": algorithm, "typ": "JWT"}).encode()) + "." + b64(json.dumps(claims).encode())
                digest = hashlib.sha384 if algorithm == "HS384" else hashlib.sha256
                return content + "." + b64(hmac.new(signing_key.encode(), content.encode(), digest).digest())

            for label, claims, algorithm, signing_key in [
                ("emisor incorrecto", dict(payload, iss="otro"), "HS256", key),
                ("audiencia incorrecta", dict(payload, aud="otra"), "HS256", key),
                ("sin expiración", {k: v for k, v in payload.items() if k != "exp"}, "HS256", key),
                ("aún no válido", dict(payload, nbf=int(time.time()) + 120, exp=int(time.time()) + 180), "HS256", key),
                ("algoritmo HS384", payload, "HS384", key),
                ("firma incorrecta", payload, "HS256", secrets.token_urlsafe(48))]:
                check(f"JWT {label}", profile(signed(claims, algorithm, signing_key)), 401)

            while time.time() <= payload["exp"] + 1:
                remaining = payload["exp"] + 1 - time.time()
                print(f"Esperando expiración del mismo JWT: {max(0, int(remaining))} s", flush=True)
                time.sleep(min(20, max(0, remaining)))
            check("Mismo JWT después de expirar", profile(token), 401)
            report = ROOT / "tests" / "resultados.txt"
            report.write_text("Pruebas HTTPS locales — " + time.strftime("%Y-%m-%d %H:%M:%S %z")
                              + "\nCertificado y clave efímeros; JWT de 1 minuto solo en el proceso de pruebas.\n"
                              + "\n".join(RESULTS) + f"\nTotal: {len(RESULTS)} comprobaciones correctas.\n")
            print(f"OK: {len(RESULTS)} comprobaciones. Registro: {report}", flush=True)
        finally:
            for process in processes:
                if process.poll() is None:
                    process.terminate()
            for process in processes:
                try:
                    process.wait(timeout=10)
                except subprocess.TimeoutExpired:
                    process.kill()
                    process.wait()
            for log in logs:
                log.close()


if __name__ == "__main__":
    main()
