using Microsoft.AspNetCore.Mvc;

namespace Semana05HttpRouting.Endpoints;

public static class ParametrosEndpoints
{
    public static void MapEjemplosParametros(this WebApplication app)
    {
        // Consultar un cliente por ID: /api/clientes/42. Solo ilustra binding.
        app.MapGet("/api/clientes/{id:int}", (int id) =>
            Results.Ok(new { id, origen = "ruta" }));

        // Filtrar vehículos: /api/vehiculos?marca=Toyota&pagina=2.
        app.MapGet("/api/vehiculos",
            (string? marca, int pagina = 1) =>
            {
                if (pagina < 1)
                    return Results.BadRequest(new { error = "La página debe ser positiva." });
                return Results.Ok(new { marca = marca ?? "Todas", pagina });
            });

        // Consultar inventario por sucursal mediante el encabezado X-Sucursal: Centro.
        app.MapGet("/api/inventario",
            ([FromHeader(Name = "X-Sucursal")] string? sucursal) =>
                Results.Ok(new { sucursal = sucursal ?? "Sin especificar" }));

        // Filtrar sucursales por ciudad. Comparar /api/sucursales y /api/sucursales?ciudad=Heredia.
        app.MapGet("/api/sucursales", (string? ciudad) =>
            Results.Ok(new { ciudad = ciudad ?? "Todas" }));

        app.MapGet("/api/sucursales/{ciudad}", (string? ciudad) =>
            Results.Ok(new { ciudad = ciudad ?? "Todas" }));

        // Consultar un vehículo indicando moneda: /api/vehiculos/8?moneda=USD. No convierte precios.
        app.MapGet("/api/vehiculos/{id:int}",
            ([FromRoute] int id, [FromQuery] string? moneda) =>
                Results.Ok(new { id, moneda = moneda ?? "CRC" }));
    }
}
