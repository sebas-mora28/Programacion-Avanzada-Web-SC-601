using System.Runtime.CompilerServices;
using System.Threading.Channels;
using System.Net.ServerSentEvents;
using SseDemo.Models;

namespace SseDemo.Services;

// Demo de una sola instancia, con datos en memoria y una suscripción por pestaña.
public sealed class PedidoService
{
    private readonly object _gate = new();
    private readonly Dictionary<Guid, Channel<Pedido>> _clientes = new();
    private Pedido _actual = new(42, "Recibido", 1, DateTimeOffset.UtcNow);

    public Pedido Obtener()
    {
        lock (_gate) return _actual;
    }

    public Pedido CambiarEstado(string estado)
    {
        lock (_gate)
        {
            _actual = _actual with
            {
                Estado = estado,
                Version = _actual.Version + 1,
                ActualizadoEn = DateTimeOffset.UtcNow
            };
            // Un canal por cliente permite que TODAS las pestañas reciban el cambio.
            // Un único canal compartido repartiría los eventos entre los lectores.
            foreach (var canal in _clientes.Values)
                canal.Writer.TryWrite(_actual);
            return _actual;
        }
    }

    public async IAsyncEnumerable<SseItem<Pedido>> Escuchar(
        [EnumeratorCancellation] CancellationToken ct)
    {
        var id = Guid.NewGuid();
        var canal = Channel.CreateBounded<Pedido>(new BoundedChannelOptions(32)
        {
            SingleReader = true,
            SingleWriter = false,
            // Solo necesitamos el estado más reciente, no un historial de transiciones.
            FullMode = BoundedChannelFullMode.DropOldest
        });

        lock (_gate)
        {
            _clientes.Add(id, canal);
            // Suscribir y tomar el estado bajo el mismo lock evita perder un cambio
            // entre leer el pedido y registrar la conexión.
            canal.Writer.TryWrite(_actual);
        }

        try
        {
            await foreach (var pedido in canal.Reader.ReadAllAsync(ct))
            {
                yield return new SseItem<Pedido>(pedido, eventType: "pedido")
                {
                    ReconnectionInterval = TimeSpan.FromSeconds(3)
                };
            }
        }
        finally
        {
            lock (_gate) _clientes.Remove(id);
            canal.Writer.TryComplete();
        }
    }
}
