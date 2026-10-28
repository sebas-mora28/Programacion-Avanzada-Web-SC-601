using System.Threading.Channels;

namespace PracticaTareasHttp.Services;

/// <summary>Cola FIFO de identificadores de tareas.</summary>
public sealed class TaskQueue
{
    private readonly Channel<Guid> _channel = Channel.CreateUnbounded<Guid>(
        new UnboundedChannelOptions { SingleReader = true, SingleWriter = false });

    public ValueTask EnqueueAsync(Guid id, CancellationToken cancellationToken = default) =>
        _channel.Writer.WriteAsync(id, cancellationToken);

    public IAsyncEnumerable<Guid> ReadAllAsync(CancellationToken cancellationToken) =>
        _channel.Reader.ReadAllAsync(cancellationToken);
}
