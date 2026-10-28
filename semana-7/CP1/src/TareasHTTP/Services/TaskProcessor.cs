using PracticaTareasHttp.Models;

namespace PracticaTareasHttp.Services;

/// <summary>Consume tareas en segundo plano. Los estudiantes NO deben modificarlo.</summary>
public sealed class TaskProcessor(TaskQueue queue, TaskStore store, ILogger<TaskProcessor> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await foreach (var id in queue.ReadAllAsync(stoppingToken))
            {
                try
                {
                    store.SetStatus(id, Models.TaskStatus.Processing);
                    logger.LogInformation("Procesando tarea {TaskId}", id);

                    var task = store.Get(id)
                        ?? throw new InvalidOperationException("La tarea no existe.");
                    await Task.Delay(TimeSpan.FromSeconds(task.Seconds), stoppingToken);

                    // Se simula un fallo ocasional para demostrar el estado Failed.
                    if (Random.Shared.Next(10) == 0)
                        throw new InvalidOperationException("Fallo simulado al procesar la tarea.");

                    store.SetStatus(id, Models.TaskStatus.Completed);
                    logger.LogInformation("Tarea {TaskId} completada", id);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception exception)
                {
                    logger.LogWarning(exception, "Falló la tarea {TaskId}", id);
                    store.SetStatus(id, Models.TaskStatus.Failed);
                }
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Cierre normal de la aplicación.
        }
    }
}
