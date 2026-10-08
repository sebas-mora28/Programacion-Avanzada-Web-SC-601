namespace Semana05HttpRouting.Endpoints;

public static class GetEndpoints
{
    public static void MapEjemplosGet(this WebApplication app)
    {
        // Texto plano: abrir esta URL en el navegador envía GET.
        app.MapGet("/api/get/saludo", () => "Hola, clase de Programación Avanzada Web");

        // Un objeto se serializa automáticamente a JSON.
        app.MapGet("/api/get/curso", () => Results.Ok(new
        {
            codigo = "SC-701",
            tema = "HTTP y enrutamiento",
            semana = 5
        }));

        app.MapGet("/api/get/hora", () => Results.Ok(new { utc = DateTimeOffset.UtcNow }));

        // La forma de la respuesta depende del contrato del endpoint.
        app.MapGet("/api/get/sin-contenido", () => Results.NoContent());
    }
}
