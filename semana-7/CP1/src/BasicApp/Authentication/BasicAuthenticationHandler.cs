using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using AuthLab;

namespace BasicApp.Authentication;

public sealed class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly UserService _users;
    public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger, UrlEncoder encoder, UserService users)
        : base(options, logger, encoder) => _users = users;

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {

        // Hint: Request.Headers.TryGetValue("Authorization", out var authHeader);

        // TODO B1: 
        // 1. Leer Authorization y comprobar el esquema Basic (sin distinguir mayúsculas).
        // 2. Decodificar Base64 como UTF-8; separar usuario/contraseña por el PRIMER ':'.
        // 3. No dejar que Base64 mal formado lance un 500. Rechazar datos incompletos.
        // 4. Validar con _users.ValidateCredentials. Construir identidad con claims
        // 5. NameIdentifier, Name y Role y devolver un ticket del esquema actual.
        // 6. Sin encabezado: NoResult. Credenciales mal formadas/incorrectas: Fail.
        // 7. Sustituir este retorno: la base deniega todas las solicitudes.
        return Task.FromResult(AuthenticateResult.NoResult());
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        // TODO B2: 
        // 1. Agregar WWW-Authenticate: Basic realm="SC701", charset="UTF-8".
        // 2. Mantener HTTP 401; no redirigir a una página.
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    }
}
