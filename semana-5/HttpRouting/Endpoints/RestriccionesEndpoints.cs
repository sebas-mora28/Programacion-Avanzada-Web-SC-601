namespace Semana05HttpRouting.Endpoints;

public static class RestriccionesEndpoints
{
    public static void MapEjemplosRestricciones(this WebApplication app)
    {
        // Si id no es entero, esta ruta no coincide: 404 en este proyecto.
        app.MapGet("/api/restricciones/entero/{id:int}", (int id) =>
            Results.Ok(new { id }));

        // Sin restricción: la ruta coincide, pero convertir abc a int falla: 400.
        app.MapGet("/api/restricciones/binding/{id}", (int id) =>
            Results.Ok(new { id }));

        // Restricciones encadenadas. 0 no coincide y por ello resulta en 404.
        app.MapGet("/api/restricciones/positivo/{id:int:min(1)}", (int id) =>
            Results.Ok(new { id }));

        // Validación dentro del handler: permite devolver un error explicativo.
        app.MapGet("/api/restricciones/validacion/{id:int}", (int id) =>
        {
            if (id <= 0)
                return Results.BadRequest(new { error = "El id debe ser mayor que cero." });
            return Results.Ok(new { id });
        });

        app.MapGet("/api/restricciones/guid/{id:guid}", (Guid id) =>
            Results.Ok(new { id }));

        app.MapGet("/api/restricciones/codigo/{codigo:length(6)}", (string codigo) =>
            Results.Ok(new { codigo, nota = "Se verifica longitud, no existencia." }));

        // Dos plantillas similares se desambiguan por tipo de segmento.
        app.MapGet("/api/restricciones/identificador/{valor:int}", (int valor) =>
            Results.Ok(new { valor, tipo = "entero" }));
        app.MapGet("/api/restricciones/identificador/{valor:guid}", (Guid valor) =>
            Results.Ok(new { valor, tipo = "guid" }));

        // Un literal tiene precedencia sobre un parámetro general.
        app.MapGet("/api/restricciones/catalogo/{texto}", (string texto) =>
            Results.Ok(new { ruta = "parametrizada", texto }));
        app.MapGet("/api/restricciones/catalogo/destacados", () =>
            Results.Ok(new { ruta = "literal", mensaje = "Vehículos destacados" }));
    }
}
