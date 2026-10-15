using EFCore_Semana6.Data;
using EFCore_Semana6.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;

namespace EFCore_Semana6.Endpoints;

public static class ReasignacionesEndpoints
{
    public static RouteGroupBuilder MapReasignacionesEndpoints(
        this IEndpointRouteBuilder app)
    {
        var grupo = app.MapGroup("/api/tareas");

        // Opción 1: EF Core realiza el UPDATE y controla la transacción.
        grupo.MapPost("/reasignar-con-transaccion", async (
            ReasignacionRequest request,
            AppDbContext db) =>
        {
            var error = await Validar(request, db);
            if (error is not null)
                return error;

            await using var transaccion = await db.Database.BeginTransactionAsync();

            try
            {
                var tareas = await db.Tareas
                    .Where(t => t.ResponsableId == request.UsuarioOrigenId)
                    .ToListAsync();

                foreach (var tarea in tareas)
                    tarea.ResponsableId = request.UsuarioDestinoId;

                await db.SaveChangesAsync();
                await transaccion.CommitAsync();

                return Results.Ok(new
                {
                    mensaje = "Tareas reasignadas con una transacción de EF Core.",
                    tareasReasignadas = tareas.Count
                });
            }
            catch (Exception)
            {
                await transaccion.RollbackAsync();

                return Results.Problem(
                    title: "No fue posible reasignar las tareas.",
                    detail: "La transacción fue revertida con RollbackAsync.",
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        });

        // Opción 2: el endpoint solamente llama al procedimiento almacenado.
        grupo.MapPost("/reasignar-con-procedimiento", async (
            ReasignacionRequest request,
            AppDbContext db) =>
        {
            var usuarioOrigen = new NpgsqlParameter(
                "p_usuario_origen_id",
                NpgsqlDbType.Integer)
            {
                Value = request.UsuarioOrigenId
            };

            var usuarioDestino = new NpgsqlParameter(
                "p_usuario_destino_id",
                NpgsqlDbType.Integer)
            {
                Value = request.UsuarioDestinoId
            };

            try
            {
            await db.Database.ExecuteSqlRawAsync(
                "CALL reasignar_tareas(@p_usuario_origen_id, @p_usuario_destino_id)",
                usuarioOrigen,
                usuarioDestino);
                       
            }catch (Exception)
            {
                return Results.Problem(
                    title: "No fue posible reasignar las tareas.",
                    statusCode: StatusCodes.Status500InternalServerError);
            }

            return Results.Ok(new
            {
                mensaje = "Procedimiento almacenado ejecutado correctamente."
            });
        });

        return grupo;
    }

    private static async Task<IResult?> Validar(
        ReasignacionRequest request,
        AppDbContext db)
    {
        var errores = RequestValidator.Validar(request);

        if (request.UsuarioOrigenId == request.UsuarioDestinoId)
            errores[nameof(request.UsuarioDestinoId)] =
                ["El usuario de destino debe ser diferente al usuario de origen."];

        if (errores.Count > 0)
            return Results.ValidationProblem(errores);

        var usuariosExistentes = await db.Usuarios
            .Where(u => u.Id == request.UsuarioOrigenId || u.Id == request.UsuarioDestinoId)
            .Select(u => u.Id)
            .ToListAsync();

        if (!usuariosExistentes.Contains(request.UsuarioOrigenId) ||
            !usuariosExistentes.Contains(request.UsuarioDestinoId))
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                ["Usuarios"] = ["El usuario de origen o de destino no existe."]
            });

        return null;
    }
}
