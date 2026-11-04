using FluentApi.Data;
using FluentApi.Endpoints;
using FluentApi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Falta ConnectionStrings:DefaultConnection.");
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!db.Categorias.Any())
    {
        var categoria = new Categoria { Nombre = "Tecnología" };
        db.Productos.Add(new Producto
        {
            Nombre = "Teclado", Descripcion = "Teclado USB",
            Precio = 15000m, Categoria = categoria
        });
        db.SaveChanges();
    }
}

app.MapGet("/", () => Results.Ok(new
{
    proyecto = "FluentApi",
    endpoints = new[] { "/api/productos", "/api/categorias", "/api/etiquetas", "/api/esquema" }
}));

app.MapGet("/api/categorias", async (AppDbContext db) =>
    await db.Categorias.AsNoTracking().OrderBy(c => c.Id)
        .Select(c => new { c.Id, c.Nombre }).ToListAsync());

// Permite comprobar que las dos configuraciones generan el mismo SQL.
app.MapGet("/api/esquema", (AppDbContext db) =>
    Results.Text(db.Database.GenerateCreateScript(), "text/plain"));

app.MapProductos();
app.MapEtiquetas();
app.Run();
