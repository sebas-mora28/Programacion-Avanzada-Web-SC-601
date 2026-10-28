# CP1 resuelto

Versión resuelta de los tres proyectos de CP1, en una carpeta independiente.
Requiere .NET SDK 10. Los datos se mantienen en memoria y se pierden al reiniciar.

## Compilar y ejecutar

Desde `CP1-resuelto`:

```bash
dotnet build CP1-resuelto.slnx
```

Ejecutar cada proyecto en una terminal distinta:

```bash
dotnet run --project src/BasicApp --launch-profile http
dotnet run --project src/TareasHTTP --launch-profile http
dotnet run --project src/ErroresHTTP --launch-profile http
```

| Proyecto | URL local | Funcionalidad resuelta |
|---|---|---|
| BasicApp | http://localhost:5000 | B1, B2 y B3: autenticación, challenge y autorización por rol |
| TareasHTTP | http://localhost:5080 | RF01 y RF02: creación, validación y consulta de tareas |
| ErroresHTTP | http://localhost:5081 | Métodos HTTP, estados, validación e idempotencia |

Cada proyecto incluye `requests.http` para usar con REST Client o como referencia para Postman.

## BasicApp

- El handler reconoce Basic sin distinguir mayúsculas, decodifica Base64 y UTF-8, y divide por el primer `:`. Encabezados mal formados y credenciales incorrectas producen `401`, sin excepciones `500`.
- Los desafíos incluyen `WWW-Authenticate: Basic realm="SC701", charset="UTF-8"`.
- El perfil devuelve únicamente `id`, `username` y `role`, obtenidos de los claims.
- `/api/basic/admin` exige rol `Admin`: un estudiante autenticado recibe `403`; una petición sin autenticación recibe `401`.
- Se conservó `UserService`. Usuarios de la práctica: `admin` / `passwordAdmin` y `estudiante` / `password123`. Las solicitudes usan estas credenciales y el puerto configurado.

El perfil HTTP es para la práctica local; Basic debe utilizar HTTPS fuera de ese entorno.

## TareasHTTP

Se modificó únicamente `Program.cs` en este proyecto, conservando la infraestructura y el enunciado.
`POST /tasks` valida `seconds` entre 1 y 60 antes de crear y encolar. Devuelve inmediatamente `202`, `Location` y estado `Pending` como texto.
`GET /tasks/{id}` devuelve `200` con el estado o `404` si la tarea no existe.
El procesador suministrado pasa por `Processing` y termina en `Completed` o `Failed` (fallo aleatorio simulado).

Para verificar validaciones, enviar `{}`, `{"seconds":0}`, `{"seconds":-1}` o `{"seconds":61}`: todos devuelven `400`.
Para observar el progreso, crear `{"seconds":10}` y consultar la URL de `Location` varias veces.

## ErroresHTTP: tres problemas y sus correcciones

1. **Métodos y respuestas incorrectos.** Crear y eliminar modificaban datos mediante GET; los recursos inexistentes devolvían 200. Se usan GET para consultas, POST para creación, PUT para actualización y DELETE para eliminación. Crear devuelve 201 y Location; eliminar, 204 sin cuerpo; un recurso inexistente, 404.
2. **Falta de validación.** Se aceptaban nombres vacíos o con espacios y precios negativos. POST y PUT validan antes de modificar el almacén y responden 400 para estos datos.
3. **Actualización no idempotente.** Cada actualización sumaba el precio enviado al anterior. PUT utiliza `ProductStore.Replace`, de modo que repetir el mismo cuerpo mantiene el mismo nombre y precio.

## Verificación automática

Después de compilar, desde esta carpeta:

```bash
python3 verificar.py
```

El script requiere Python 3, inicia instancias temporales de las tres API en puertos locales asignados por el sistema y las detiene al terminar. Comprueba autenticación y roles, errores de entrada, códigos y encabezados HTTP, idempotencia y procesamiento de tareas. No necesita paquetes adicionales.
