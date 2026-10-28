# Métodos HTTP y tareas asíncronas

Proyecto base en **ASP.NET Core Minimal API (.NET 10)** para la sección de procesamiento de tareas.

## Objetivo

Implementar únicamente **dos endpoints** en `Program.cs`. Ya se entregan la cola (`Channel<Guid>`), el servicio de procesamiento (`BackgroundService`), el almacén en memoria y el modelo de tarea.

| Método | Ruta | Respuesta esperada |
|---|---|---|
| `POST` | `/tasks` | `202 Accepted`, encabezado `Location: /tasks/{id}` y JSON `{ "id": "...", "status": "Pending" }` |
| `GET` | `/tasks/{id}` | `200 OK` con `id` y `status`, o `404 Not Found` si no existe |

Los estados son `Pending`, `Processing`, `Completed` y `Failed`. Al crear una tarea, enviar un cuerpo JSON como `{ "seconds": 10 }` con `Content-Type: application/json`. La duración debe ser un número entero entre 1 y 60 segundos. Los estudiantes deben implementar la validación para que un valor fuera de ese rango o la ausencia de `seconds` devuelva `400 Bad Request`. La tarea espera su turno en la cola y luego se procesa durante el tiempo solicitado. El procesamiento puede fallar ocasionalmente de forma simulada.

## Ejecutar

1. Instalar .NET SDK 10.
2. Abrir esta carpeta en VS Code.
3. Ejecutar:

```bash
dotnet restore
dotnet run --launch-profile http
```

4. Utilizar `requests.http` con la extensión REST Client de VS Code, o Postman/curl.

## Lo que ya está implementado

- `Models/TaskJob.cs`: modelo inmutable con Id, Status y Seconds.
- `Models/CreateTaskRequest.cs`: cuerpo de la petición para especificar Seconds.
- `Models/TaskStatus.cs`: enumeración de estados.
- `Services/TaskStore.cs`: `Create(int seconds)`, `Get(Guid)` y `SetStatus(...)`.
- `Services/TaskQueue.cs`: `EnqueueAsync(Guid, CancellationToken)` y `ReadAllAsync(...)`.
- `Services/TaskProcessor.cs`: consumo automático de la cola y actualización de estados.

## Trabajo del estudiante

Modificar solamente `Program.cs`, completando los TODO de RF01 y RF02.

- [ ] RF01: Implementar `POST /tasks` y recibir el cuerpo JSON mediante `CreateTaskRequest`.
- [ ] RF01: Validar que `seconds` esté entre 1 y 60, inclusive. Si está fuera del rango o no se envía (su valor será 0), responder `400 Bad Request` con un mensaje, sin crear ni encolar la tarea.
- [ ] RF01: Crear la tarea con la duración solicitada y agregar su identificador a la cola.
- [ ] RF01: Responder `202 Accepted` con `Location: /tasks/{id}` y un JSON con `id` y `status` como texto.
- [ ] RF02: Implementar la consulta de tareas por identificador.

### Criterios de aceptación

1. `POST /tasks` crea una tarea y la encola sin esperar su finalización.
2. La respuesta es `202 Accepted` e incluye `Location` y un cuerpo JSON con el identificador y estado inicial.
3. `GET /tasks/{id}` devuelve el estado de una tarea existente con `200 OK`.
4. `GET /tasks/{id}` devuelve `404 Not Found` si no existe.
5. Al consultar repetidamente una tarea se observa su progreso y finalmente `Completed` o `Failed`.
6. `POST /tasks` rechaza duraciones fuera de 1 a 60 segundos o la ausencia de `seconds` con `400 Bad Request`, sin crear ni encolar una tarea.

> Los datos se guardan solo en memoria; reiniciar la aplicación elimina las tareas. No se requiere base de datos.
