using EFCore_Semana6.Data;
using EFCore_Semana6.Models;
using Microsoft.EntityFrameworkCore;

namespace EFCore_Semana6.Endpoints;

public static class UsuariosEndpoints
{
    public static RouteGroupBuilder MapUsuariosEndpoints(this IEndpointRouteBuilder app)
    {
        var grupo = app.MapGroup("/api/usuarios");

        grupo.MapGet("/", async (AppDbContext db) =>
        {
            var usuarios = await db.Usuarios
                .AsNoTracking()
                .OrderBy(u => u.Nombre)
                .ToListAsync();

            return Results.Ok(usuarios);
        });

        grupo.MapPost("/", async (UsuarioRequest request, AppDbContext db) =>
        {
            var errores = RequestValidator.Validar(request);
            if (errores.Count > 0)
                return Results.ValidationProblem(errores);

            if (await db.Usuarios.AnyAsync(u => u.Email == request.Email))
                return Results.Conflict(new { mensaje = "Ya existe un usuario con ese correo." });

            var usuario = new Usuario { Nombre = request.Nombre, Email = request.Email };
            db.Usuarios.Add(usuario);
            await db.SaveChangesAsync();

            return Results.Created($"/api/usuarios/{usuario.Id}", usuario);
        });

        grupo.MapPut("/{id:int}", async (int id, UsuarioRequest request, AppDbContext db) =>
        {
            var errores = RequestValidator.Validar(request);
            if (errores.Count > 0)
                return Results.ValidationProblem(errores);

            var usuario = await db.Usuarios.FindAsync(id);
            if (usuario is null)
                return Results.NotFound();

            if (await db.Usuarios.AnyAsync(u => u.Id != id && u.Email == request.Email))
                return Results.Conflict(new { mensaje = "Ya existe un usuario con ese correo." });

            usuario.Nombre = request.Nombre;
            usuario.Email = request.Email;
            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        grupo.MapDelete("/{id:int}", async (int id, AppDbContext db) =>
        {
            var usuario = await db.Usuarios.FindAsync(id);
            if (usuario is null)
                return Results.NotFound();

            db.Usuarios.Remove(usuario);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        return grupo;
    }
}
