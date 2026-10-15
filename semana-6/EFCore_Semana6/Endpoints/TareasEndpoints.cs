using EFCore_Semana6.Data;
using EFCore_Semana6.Models;
using Microsoft.EntityFrameworkCore;

namespace EFCore_Semana6.Endpoints;

public static class TareasEndpoints
{
    public static RouteGroupBuilder MapTareasEndpoints(this IEndpointRouteBuilder app)
    {
        var grupo = app.MapGroup("/api/tareas");

        // READ: EF Core convierte LINQ a una consulta SQL.
        grupo.MapGet("/", async (AppDbContext db) =>
            Results.Ok(await db.Tareas
                .AsNoTracking()
                .OrderBy(t => t.Id)
                .ToListAsync()));

        grupo.MapGet("/{id:int}", async (int id, AppDbContext db) =>
        {
            var tarea = await db.Tareas.FindAsync(id);
            return tarea is null ? Results.NotFound() : Results.Ok(tarea);
        });

        // INNER JOIN con Categoria y LEFT JOIN con Usuario porque el responsable es opcional.
        grupo.MapGet("/detalle", async (AppDbContext db) =>
        {
            var consulta =
                from tarea in db.Tareas.AsNoTracking()
                join categoria in db.Categorias on tarea.CategoriaId equals categoria.Id
                join usuario in db.Usuarios on tarea.ResponsableId equals usuario.Id into responsables
                from responsable in responsables.DefaultIfEmpty()
                orderby tarea.Id
                select new TareaDetalle(
                    tarea.Id,
                    tarea.Titulo,
                    tarea.Descripcion,
                    tarea.Completada,
                    tarea.Prioridad,
                    tarea.FechaCreacion,
                    categoria.Id,
                    categoria.Nombre,
                    responsable == null ? null : responsable.Id,
                    responsable == null ? null : responsable.Nombre,
                    responsable == null ? null : responsable.Email);

            return Results.Ok(await consulta.ToListAsync());
        });

        // LEFT JOIN agrupado: también incluye categorías que todavía no tienen tareas.
        grupo.MapGet("/resumen-por-categoria", async (AppDbContext db) =>
        {
            var consulta =
                from categoria in db.Categorias.AsNoTracking()
                join tarea in db.Tareas on categoria.Id equals tarea.CategoriaId into tareas
                orderby categoria.Nombre
                select new
                {
                    categoria.Id,
                    categoria.Nombre,
                    TotalTareas = tareas.Count(),
                    Completadas = tareas.Count(t => t.Completada),
                    Pendientes = tareas.Count(t => !t.Completada)
                };

            return Results.Ok(await consulta.ToListAsync());
        });

        // CREATE: Add marca la entidad como Added; SaveChangesAsync ejecuta INSERT.
        grupo.MapPost("/", async (TareaRequest request, AppDbContext db) =>
        {
            var errores = RequestValidator.Validar(request);
            if (errores.Count > 0)
                return Results.ValidationProblem(errores);

            var errorRelacion = await ValidarRelaciones(request, db);
            if (errorRelacion is not null)
                return errorRelacion;

            var tarea = new Tarea
            {
                Titulo = request.Titulo,
                Descripcion = request.Descripcion,
                Completada = request.Completada,
                Prioridad = request.Prioridad,
                CategoriaId = request.CategoriaId,
                ResponsableId = request.ResponsableId
            };

            db.Tareas.Add(tarea);
            await db.SaveChangesAsync();

            return Results.Created($"/api/tareas/{tarea.Id}", tarea);
        });

        // UPDATE: EF Core rastrea cambios sobre la entidad recuperada.
        grupo.MapPut("/{id:int}", async (int id, TareaRequest request, AppDbContext db) =>
        {
            var errores = RequestValidator.Validar(request);
            if (errores.Count > 0)
                return Results.ValidationProblem(errores);

            var errorRelacion = await ValidarRelaciones(request, db);
            if (errorRelacion is not null)
                return errorRelacion;

            var tarea = await db.Tareas.FindAsync(id);
            if (tarea is null)
                return Results.NotFound();

            tarea.Titulo = request.Titulo;
            tarea.Descripcion = request.Descripcion;
            tarea.Completada = request.Completada;
            tarea.Prioridad = request.Prioridad;
            tarea.CategoriaId = request.CategoriaId;
            tarea.ResponsableId = request.ResponsableId;

            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        // DELETE: Remove marca la entidad como Deleted.
        grupo.MapDelete("/{id:int}", async (int id, AppDbContext db) =>
        {
            var tarea = await db.Tareas.FindAsync(id);
            if (tarea is null)
                return Results.NotFound();

            db.Tareas.Remove(tarea);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        // Función de PostgreSQL creada en la migración inicial.
        // FromSqlInterpolated parametriza el argumento; evita concatenar SQL.
        grupo.MapGet("/por-estado", async (bool completada, AppDbContext db) =>
        {
            var tareas = await db.Tareas
                .FromSqlInterpolated($"SELECT * FROM obtener_tareas_por_estado({completada})")
                .AsNoTracking()
                .ToListAsync();

            return Results.Ok(tareas);
        });

        return grupo;
    }

    private static async Task<IResult?> ValidarRelaciones(TareaRequest request, AppDbContext db)
    {
        if (!await db.Categorias.AnyAsync(c => c.Id == request.CategoriaId))
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                [nameof(request.CategoriaId)] = ["La categoría indicada no existe."]
            });

        if (request.ResponsableId.HasValue &&
            !await db.Usuarios.AnyAsync(u => u.Id == request.ResponsableId.Value))
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                [nameof(request.ResponsableId)] = ["El responsable indicado no existe."]
            });

        return null;
    }
}
