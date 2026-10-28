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
            status = "Autenticación Basic lista"
        }))
            .AllowAnonymous();

        // El handler de autenticación valida las credenciales antes de ejecutar el endpoint.
        endpoints.MapGet("/api/basic/profile", (ClaimsPrincipal user) =>
        {
            return Results.Ok(new ProfileResponse(
                user.FindFirstValue(ClaimTypes.NameIdentifier)!,
                user.FindFirstValue(ClaimTypes.Name)!,
                user.FindFirstValue(ClaimTypes.Role)!));
        })
            .RequireAuthorization();

        // Autenticado sin rol Admin: 403. Sin autenticación válida: challenge 401 (B2).
        endpoints.MapGet("/api/basic/admin", () => Results.Ok(new
        {
            message = "Acceso autorizado para administradores."
        }))
            .RequireAuthorization(policy => policy.RequireAuthenticatedUser().RequireRole("Admin"));

        return endpoints;
    }
}
