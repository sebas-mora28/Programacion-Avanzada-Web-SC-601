using Semana05HttpRouting.Models;
using Semana05HttpRouting.Services;

namespace Semana05HttpRouting.Endpoints;

public static class AutosEndpoints
{
    public static void MapAutos(this WebApplication app)
    {
        // MapGroup evita repetir el prefijo en cada registro.
        var grupo = app.MapGroup("/api/autos");

        // CatalogoAutos viene de DI: no se obtiene del cuerpo ni de la URL.
        grupo.MapGet("", (CatalogoAutos catalogo, string? marca, decimal? precioMaximo) =>
        {
            if (precioMaximo is < 0)
                return Results.BadRequest(new { error = "El precio máximo no puede ser negativo." });

            return Results.Ok(catalogo.Buscar(marca, precioMaximo));
        });

        grupo.MapGet("/api/autos/{id:int}", (int id, CatalogoAutos catalogo) =>
        {
            if (id <= 0)
                return Results.BadRequest(new { error = "El id debe ser positivo." });
            var auto = catalogo.Obtener(id);
            return auto is null ? Results.NotFound() : Results.Ok(auto);
        });

        grupo.MapPost("", (CrearAuto entrada, CatalogoAutos catalogo) =>
        {
            var errores = new Dictionary<string, string[]>();
            if (string.IsNullOrWhiteSpace(entrada.Marca) || entrada.Marca.Length > 50)
                errores["marca"] = ["Indique una marca de 1 a 50 caracteres."];
            if (string.IsNullOrWhiteSpace(entrada.Modelo) || entrada.Modelo.Length > 80)
                errores["modelo"] = ["Indique un modelo de 1 a 80 caracteres."];
            if (entrada.Precio <= 0 || entrada.Precio > 1_000_000_000m)
                errores["precio"] = ["El precio debe ser positivo y no superar 1000000000."];
            if (errores.Count > 0)
                return Results.ValidationProblem(errores);

            var auto = catalogo.Crear(entrada);
            return Results.Created($"/api/autos/{auto.Id}", auto);
        });
    }
}
