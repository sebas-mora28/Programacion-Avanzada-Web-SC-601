using PracticaTareasHttp.Models;
using PracticaTareasHttp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<TaskStore>();
builder.Services.AddSingleton<TaskQueue>();
builder.Services.AddHostedService<TaskProcessor>();

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new { message = "API de tareas lista" }));

// TODO RF01: POST /tasks
// - Recibir el cuerpo JSON mediante CreateTaskRequest.
// - Verificar que request.Seconds esté entre 1 y 60, inclusive.
// - Si está fuera del rango o no se envió seconds (su valor será 0),
//   responder 400 Bad Request con un mensaje y no crear ni encolar la tarea.
// - Crear una tarea usando TaskStore.Create(request.Seconds).
// - Agregar su Id a TaskQueue mediante EnqueueAsync(...).
// - Responder 202 Accepted, incluyendo Location: /tasks/{id}
//   y un JSON con id y status (como texto).
// Pista: Results.Accepted(location, value).

// TODO RF02: GET /tasks/{id}
// - Consultar TaskStore.Get(id).
// - Si no existe, responder 404 Not Found.
// - Si existe, responder 200 OK con id y status.

app.Run();
