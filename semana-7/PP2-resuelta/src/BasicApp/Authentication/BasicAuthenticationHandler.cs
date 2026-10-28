using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using AuthLab;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace BasicApp.Authentication;

public sealed class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly UserService _users;
    public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger, UrlEncoder encoder, UserService users)
        : base(options, logger, encoder) => _users = users;

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
            return Task.FromResult(AuthenticateResult.NoResult());

        if (!AuthenticationHeaderValue.TryParse(Request.Headers.Authorization.ToString(), out var header)
            || !string.Equals(header.Scheme, "Basic", StringComparison.OrdinalIgnoreCase)
            || string.IsNullOrWhiteSpace(header.Parameter))
            return Task.FromResult(AuthenticateResult.Fail("Encabezado Basic inválido."));

        string credentials;
        try
        {
            credentials = new UTF8Encoding(false, true).GetString(Convert.FromBase64String(header.Parameter));
        }
        catch (Exception ex) when (ex is FormatException or DecoderFallbackException)
        {
            return Task.FromResult(AuthenticateResult.Fail("Credenciales mal formadas."));
        }

        var separator = credentials.IndexOf(':');
        if (separator <= 0 || separator == credentials.Length - 1)
            return Task.FromResult(AuthenticateResult.Fail("Credenciales incompletas."));

        var user = _users.ValidateCredentials(credentials[..separator], credentials[(separator + 1)..]);
        if (user is null)
            return Task.FromResult(AuthenticateResult.Fail("Credenciales incorrectas."));

        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        }, Scheme.Name);
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.Headers.WWWAuthenticate = "Basic realm=\"SC701\", charset=\"UTF-8\"";
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    }
}
