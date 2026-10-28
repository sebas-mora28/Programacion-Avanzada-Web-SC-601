namespace PracticaTareasHttp.Models;

public sealed record TaskJob(Guid Id, TaskStatus Status, int Seconds);
