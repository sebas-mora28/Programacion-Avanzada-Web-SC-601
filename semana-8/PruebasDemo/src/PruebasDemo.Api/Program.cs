using Microsoft.EntityFrameworkCore;
using PruebasDemo.Api.Data;
using PruebasDemo.Api.Endpoints;
using PruebasDemo.Api.Repositories;
using PruebasDemo.Api.Services;

var builder = WebApplication.CreateBuilder(args);
// .NET 10: valida los atributos del DTO antes de ejecutar el handler de Minimal API.
builder.Services.AddValidation();
// Mantiene las respuestas de validación con el formato application/problem+json.
builder.Services.AddProblemDetails();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Falta ConnectionStrings:DefaultConnection.");
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

// DbContext y repositorio viven durante una solicitud (Scoped); no se comparten como Singleton.
// Las unitarias sustituyen el repositorio con Moq; las de integración solo cambian el proveedor.
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<CotizacionService>();

var app = builder.Build();
// El esquema PostgreSQL se prepara con dotnet ef database update, no al iniciar la API.
// La fábrica de integración crea por separado su esquema temporal de SQLite.
app.MapCotizaciones();
app.MapGet("/", () => Results.Ok(new
{
    mensaje = "Ejemplo de pruebas unitarias y de integración",
    endpoint = "POST /api/cotizaciones",
    ejemplo = new { productoId = 1, cantidad = 5 }
}));
app.Run();

// Hace visible el punto de entrada para WebApplicationFactory<Program> desde otro proyecto.
public partial class Program { }
