using System.Security.Claims;
using AuthLab;

namespace BasicApp.Endpoints;

public static class BasicEndpoints
{
    public static IEndpointRouteBuilder MapBasicEndpoints(this IEndpointRouteBuilder endpoints)
    {
        // Endpoint público para comprobar que la aplicación está ejecutándose.
        endpoints.MapGet("/", () => Results.Ok(new
        {
            app = "BasicApp",
            status = "Base: completar B1, B2 y B3"
        }))
            .AllowAnonymous();

        // El handler de autenticación valida las credenciales antes de ejecutar el endpoint.
        endpoints.MapGet("/api/basic/profile", (ClaimsPrincipal user) =>
        {
            // TODO B3: devolver el perfil del usuario que ya autenticó el handler:
            // 1. Leer del parámetro user los claims ClaimTypes.NameIdentifier,
            //    ClaimTypes.Name y ClaimTypes.Role.
            // 2. Usar sus valores para crear un ProfileResponse (Id, Username y Role).
            // 3. Devolver el objeto con Results.Ok, que responde HTTP 200.
            //    Ejemplo para admin: { "id": "1", "username": "admin", "role": "Admin" }.
            // No validar credenciales otra vez ni devolver el objeto AppUser o su password.
            // Mantener RequireAuthorization y reemplazar la respuesta 501 provisional.
            return Results.Problem(
                statusCode: StatusCodes.Status501NotImplemented,
                title: "Pendiente B3: devolver el perfil del usuario autenticado.");
        })
            .RequireAuthorization();

        // Autenticado sin rol Admin: 403. Sin autenticación válida: challenge 401 (B2).
        endpoints.MapGet("/api/basic/admin", () => Results.Ok(new
        {
            message = "Acceso autorizado para administradores."
        }))
            // TODO B3: requerir autenticación y rol Admin para este endpoint.
            .RequireAuthorization();

        return endpoints;
    }
}
