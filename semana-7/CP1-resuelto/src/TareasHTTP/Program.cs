using PracticaTareasHttp.Models;
using PracticaTareasHttp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<TaskStore>();
builder.Services.AddSingleton<TaskQueue>();
builder.Services.AddHostedService<TaskProcessor>();

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new { message = "API de tareas lista" }));

app.MapPost("/tasks", async (CreateTaskRequest request, TaskStore store, TaskQueue queue) =>
{
    if (request.Seconds is < 1 or > 60)
        return Results.BadRequest(new { error = "seconds debe ser un entero entre 1 y 60." });

    var task = store.Create(request.Seconds);
    await queue.EnqueueAsync(task.Id);
    // El registro original conserva Pending aunque el procesador ya haya comenzado.
    return Results.Accepted($"/tasks/{task.Id}", new { id = task.Id, status = task.Status.ToString() });
});

app.MapGet("/tasks/{id:guid}", (Guid id, TaskStore store) =>
{
    var task = store.Get(id);
    return task is null
        ? Results.NotFound(new { error = "La tarea no existe." })
        : Results.Ok(new { id = task.Id, status = task.Status.ToString() });
});

app.Run();
