using FluentApi.Data;
using FluentApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FluentApi.Endpoints;

public static class ProductosEndpoints
{
    public static void MapProductos(this WebApplication app)
    {
        var grupo = app.MapGroup("/api/productos");

        // La proyección consulta ambas relaciones: Categoria (1:N) y Etiquetas (M:N).
        // EF traduce estas navegaciones a SQL; aquí no hace falta Include.
        // Seleccionar solo Id y Nombre de las etiquetas evita ciclos en el JSON.
        grupo.MapGet("/", async (AppDbContext db) =>
            await db.Productos.AsNoTracking().OrderBy(p => p.Id)
                .Select(p => new
                {
                    p.Id, p.Nombre, p.Descripcion, p.Precio, p.CategoriaId,
                    Categoria = p.Categoria.Nombre,
                    Etiquetas = p.Etiquetas.OrderBy(e => e.Id)
                        .Select(e => new { e.Id, e.Nombre }).ToList()
                }).ToListAsync());

        grupo.MapGet("/{id:int}", async (int id, AppDbContext db) =>
        {
            var producto = await db.Productos.AsNoTracking()
                .Where(p => p.Id == id)
                .Select(p => new
                {
                    p.Id, p.Nombre, p.Descripcion, p.Precio, p.CategoriaId,
                    Categoria = p.Categoria.Nombre,
                    Etiquetas = p.Etiquetas.OrderBy(e => e.Id)
                        .Select(e => new { e.Id, e.Nombre }).ToList()
                }).FirstOrDefaultAsync();
            return producto is null ? Results.NotFound() : Results.Ok(producto);
        });

        grupo.MapPost("/", async (ProductoRequest entrada, AppDbContext db) =>
        {
            var errores = entrada.Validar();
            if (errores.Count > 0) return Results.ValidationProblem(errores);
            if (!await db.Categorias.AnyAsync(c => c.Id == entrada.CategoriaId))
                return Results.BadRequest(new { error = "La categoría no existe." });

            var producto = new Producto();
            Aplicar(entrada, producto);
            db.Productos.Add(producto);
            await db.SaveChangesAsync();
            return Results.Created($"/api/productos/{producto.Id}", new { producto.Id });
        });

        grupo.MapPut("/{id:int}", async (int id, ProductoRequest entrada, AppDbContext db) =>
        {
            var producto = await db.Productos.FindAsync(id);
            if (producto is null) return Results.NotFound();
            var errores = entrada.Validar();
            if (errores.Count > 0) return Results.ValidationProblem(errores);
            if (!await db.Categorias.AnyAsync(c => c.Id == entrada.CategoriaId))
                return Results.BadRequest(new { error = "La categoría no existe." });

            Aplicar(entrada, producto);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        grupo.MapDelete("/{id:int}", async (int id, AppDbContext db) =>
        {
            var producto = await db.Productos.FindAsync(id);
            if (producto is null) return Results.NotFound();
            db.Productos.Remove(producto);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });
    }

    private static void Aplicar(ProductoRequest entrada, Producto producto)
    {
        producto.Nombre = entrada.Nombre.Trim();
        producto.Descripcion = entrada.Descripcion;
        producto.Precio = entrada.Precio;
        producto.CategoriaId = entrada.CategoriaId;
    }
}
