using System.Collections.Concurrent;
using PracticaTareasHttp.Models;

namespace PracticaTareasHttp.Services;

/// <summary>Almacén en memoria, seguro para accesos concurrentes.</summary>
public sealed class TaskStore
{
    private readonly ConcurrentDictionary<Guid, TaskJob> _jobs = new();

    public TaskJob Create(int seconds)
    {
        var job = new TaskJob(Guid.NewGuid(), Models.TaskStatus.Pending, seconds);
        _jobs[job.Id] = job;
        return job;
    }

    public TaskJob? Get(Guid id) => _jobs.GetValueOrDefault(id);

    public void SetStatus(Guid id, Models.TaskStatus status)
    {
        // AddOrUpdate mantiene el reemplazo atómico del registro inmutable.
        _jobs.AddOrUpdate(id,
            _ => throw new InvalidOperationException("La tarea no existe."),
            (_, current) => current with { Status = status });
    }
}
