using System.Runtime.CompilerServices;
using SseDemo.Models;
using SseDemo.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<PedidoService>();
var app = builder.Build();

// La página y la API usan el mismo origen. No se necesita CORS para esta demo.
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/pedidos/42", (PedidoService pedidos) => pedidos.Obtener());

// GET mantiene una respuesta abierta. El resultado escribe text/event-stream.
app.MapGet("/eventos", (PedidoService pedidos, HttpContext http, CancellationToken ct) =>
{
    http.Response.Headers.CacheControl = "no-cache";
    return TypedResults.ServerSentEvents(pedidos.Escuchar(ct));
});

// Las acciones del navegador viajan por HTTP normal, no por el canal SSE.
app.MapPost("/api/pedidos/42/estado", IResult (
    CambiarEstadoRequest request, PedidoService pedidos) =>
{
    string[] permitidos = ["Recibido", "En preparación", "En camino", "Entregado"];
    if (request.Estado is null || !permitidos.Contains(request.Estado))
        return Results.BadRequest(new { error = "Seleccione un estado válido." });

    return Results.Ok(pedidos.CambiarEstado(request.Estado));
});

// Ejemplo mínimo: produce un valor cada tres segundos para cada conexión.
app.MapGet("/eventos/reloj", (CancellationToken ct) =>
    TypedResults.ServerSentEvents(ProducirReloj(ct), eventType: "reloj"));

app.Run();

static async IAsyncEnumerable<object> ProducirReloj(
    [EnumeratorCancellation] CancellationToken ct)
{
    while (!ct.IsCancellationRequested)
    {
        yield return new { hora = DateTimeOffset.UtcNow };
        await Task.Delay(3000, ct);
    }
}
