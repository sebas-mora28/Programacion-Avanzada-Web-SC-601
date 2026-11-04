using PruebasDemo.Api.Models;
using PruebasDemo.Api.Services;

namespace PruebasDemo.Api.Endpoints;

public static class CotizacionesEndpoints
{
    // Agrupamos el endpoint fuera de Program para mantener clara la configuración de la aplicación.
    public static void MapCotizaciones(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/cotizaciones", async Task<IResult> (
            CotizacionRequest request,
            CotizacionService service,
            CancellationToken cancellationToken) =>
        {
            // Minimal API obtiene request del JSON y service de la inyección de dependencias.
            // El token se vincula a la cancelación de la solicitud HTTP.
            // AddValidation procesa los atributos Range antes de ejecutar este handler;
            // una entrada inválida produce HTTP 400 con los errores por propiedad.
            var cotizacion = await service.CotizarAsync(
                request.ProductoId, request.Cantidad, cancellationToken);

            if (cotizacion is null)
                return Results.NotFound(new { mensaje = "El producto no existe." });

            return Results.Ok(cotizacion);
        });
    }
}
