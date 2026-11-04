using Tradicional.Data;
using Tradicional.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Tradicional.Endpoints;

public static class EtiquetasEndpoints
{
    public static void MapEtiquetas(this WebApplication app)
    {
        app.MapGet("/api/etiquetas", async (AppDbContext db) =>
            await db.Etiquetas.AsNoTracking().OrderBy(e => e.Id)
                .Select(e => new
                {
                    e.Id, e.Nombre,
                    Productos = e.Productos.OrderBy(p => p.Id)
                        .Select(p => new { p.Id, p.Nombre }).ToList()
                }).ToListAsync());

        app.MapGet("/api/etiquetas/{id:int}", async (int id, AppDbContext db) =>
        {
            var etiqueta = await db.Etiquetas.AsNoTracking().Where(e => e.Id == id)
                .Select(e => new
                {
                    e.Id, e.Nombre,
                    Productos = e.Productos.OrderBy(p => p.Id)
                        .Select(p => new { p.Id, p.Nombre }).ToList()
                }).FirstOrDefaultAsync();
            return etiqueta is null ? Results.NotFound() : Results.Ok(etiqueta);
        });

        app.MapPost("/api/etiquetas", async (EtiquetaRequest entrada, AppDbContext db) =>
        {
            var nombre = entrada.Nombre?.Trim();
            if (string.IsNullOrEmpty(nombre) || nombre.Length > 50)
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    ["Nombre"] = ["El nombre es obligatorio y admite hasta 50 caracteres."]
                });

            var etiqueta = new Etiqueta { Nombre = nombre };
            db.Etiquetas.Add(etiqueta);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException
                   { SqlState: PostgresErrorCodes.UniqueViolation })
            {
                return Results.Conflict(new { error = "Ya existe una etiqueta con ese nombre." });
            }
            return Results.Created($"/api/etiquetas/{etiqueta.Id}", new { etiqueta.Id, etiqueta.Nombre });
        });

        app.MapPut("/api/productos/{id:int}/etiquetas/{etiquetaId:int}",
            async (int id, int etiquetaId, AppDbContext db) =>
        {
            var producto = await db.Productos.Include(p => p.Etiquetas)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (producto is null) return Results.NotFound();
            var etiqueta = await db.Etiquetas.FindAsync(etiquetaId);
            if (etiqueta is null) return Results.NotFound();

            if (producto.Etiquetas.All(e => e.Id != etiquetaId))
            {
                producto.Etiquetas.Add(etiqueta);
                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateException ex) when (ex.InnerException is PostgresException
                       { SqlState: PostgresErrorCodes.UniqueViolation })
                {
                    // Otra solicitud agregó la misma asociación: PUT sigue siendo idempotente.
                }
            }
            return Results.NoContent();
        });

        app.MapDelete("/api/productos/{id:int}/etiquetas/{etiquetaId:int}",
            async (int id, int etiquetaId, AppDbContext db) =>
        {
            var producto = await db.Productos.Include(p => p.Etiquetas)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (producto is null || !await db.Etiquetas.AnyAsync(e => e.Id == etiquetaId))
                return Results.NotFound();

            var etiqueta = producto.Etiquetas.FirstOrDefault(e => e.Id == etiquetaId);
            if (etiqueta is not null)
            {
                producto.Etiquetas.Remove(etiqueta);
                await db.SaveChangesAsync();
            }
            return Results.NoContent();
        });
    }
}
