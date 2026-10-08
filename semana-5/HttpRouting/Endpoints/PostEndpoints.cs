using Semana05HttpRouting.Models;

namespace Semana05HttpRouting.Endpoints;

public static class PostEndpoints
{
    public static void MapEjemplosPost(this WebApplication app)
    {
        // El DTO complejo se recibe del cuerpo JSON.
        app.MapPost("/api/eco", (MensajeEntrada entrada) =>
        {
            if (string.IsNullOrWhiteSpace(entrada.Mensaje))
                return Results.BadRequest(new { error = "El mensaje es obligatorio." });

            return Results.Ok(new { recibido = entrada.Mensaje.Trim() });
        });

        // POST también puede procesar datos sin crear un recurso persistente.
        app.MapPost("/api/cotizacion", (CotizacionEntrada entrada) =>
        {
            if (entrada.Precio <= 0 || entrada.Precio > 1_000_000_000m)
                return Results.BadRequest(new { error = "Precio entre 0 (exclusivo) y 1000000000." });
            if (entrada.PorcentajeDescuento < 0 || entrada.PorcentajeDescuento > 100)
                return Results.BadRequest(new { error = "Descuento entre 0 y 100." });

            var descuento = decimal.Round(
                entrada.Precio * entrada.PorcentajeDescuento / 100m, 2);
            return Results.Ok(new
            {
                precio = entrada.Precio,
                descuento,
                total = entrada.Precio - descuento
            });
        });
    }
}
