var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Reescribe / a /index.html. Debe ir ANTES de UseStaticFiles.
app.UseDefaultFiles();
// Publica archivos de wwwroot: /css/site.css, /js/site.js, etc.
// Elegimos este middleware para observar cambios en archivos durante la clase.
app.UseStaticFiles();

// Estos datos son de demostración, no una base de datos.
Pelicula[] peliculas =
[
    new(1, "Viaje a las estrellas", "Ciencia ficción", 125),
    new(2, "Una tarde inesperada", "Comedia", 98),
    new(3, "El último tren", "Drama", 112)
];

// Endpoint dinámico: recibe un filtro y genera una respuesta JSON.
app.MapGet("/api/peliculas", (string? genero) =>
{
    var resultado = peliculas.Where(p => string.IsNullOrWhiteSpace(genero)
        || string.Equals(p.Genero, genero, StringComparison.OrdinalIgnoreCase));
    return Results.Ok(resultado);
});

app.MapGet("/api/peliculas/{id:int}", (int id) =>
{
    var pelicula = peliculas.FirstOrDefault(p => p.Id == id);
    return pelicula is null ? Results.NotFound() : Results.Ok(pelicula);
});

// Cada llamada genera una hora nueva: contraste con /datos/ejemplo.json.
app.MapGet("/api/hora", () => Results.Ok(new { horaUtc = DateTimeOffset.UtcNow }));

app.Run();

public record Pelicula(int Id, string Titulo, string Genero, int Duracion);
