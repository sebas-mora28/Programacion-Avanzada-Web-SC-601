using EFCore_Semana6.Data;
using EFCore_Semana6.Models;
using Microsoft.EntityFrameworkCore;

namespace EFCore_Semana6.Endpoints;

public static class CategoriasEndpoints
{
    public static RouteGroupBuilder MapCategoriasEndpoints(this IEndpointRouteBuilder app)
    {
        var grupo = app.MapGroup("/api/categorias");

        grupo.MapGet("/", async (AppDbContext db) =>
            Results.Ok(await db.Categorias.AsNoTracking().OrderBy(c => c.Nombre).ToListAsync()));

        grupo.MapPost("/", async (CategoriaRequest request, AppDbContext db) =>
        {
            var errores = RequestValidator.Validar(request);
            if (errores.Count > 0)
                return Results.ValidationProblem(errores);

            if (await db.Categorias.AnyAsync(c => c.Nombre == request.Nombre))
                return Results.Conflict(new { mensaje = "Ya existe una categoría con ese nombre." });

            var categoria = new Categoria { Nombre = request.Nombre };
            db.Categorias.Add(categoria);

            await db.SaveChangesAsync();

            return Results.Created($"/api/categorias/{categoria.Id}", categoria);
        });

        grupo.MapPut("/{id:int}", async (int id, CategoriaRequest request, AppDbContext db) =>
        {
            var errores = RequestValidator.Validar(request);
            if (errores.Count > 0)
                return Results.ValidationProblem(errores);

            var categoria = await db.Categorias.FindAsync(id);
            if (categoria is null)
                return Results.NotFound();

            if (await db.Categorias.AnyAsync(c => c.Id != id && c.Nombre == request.Nombre))
                return Results.Conflict(new { mensaje = "Ya existe una categoría con ese nombre." });

            categoria.Nombre = request.Nombre;
            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        grupo.MapDelete("/{id:int}", async (int id, AppDbContext db) =>
        {
            var categoria = await db.Categorias.FindAsync(id);
            if (categoria is null)
                return Results.NotFound();

            if (await db.Tareas.AnyAsync(t => t.CategoriaId == id))
                return Results.Conflict(new
                {
                    mensaje = "No se puede eliminar una categoría que tiene tareas asociadas."
                });

            db.Categorias.Remove(categoria);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        return grupo;
    }
}
