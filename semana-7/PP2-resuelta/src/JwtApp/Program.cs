using AuthLab;
using JwtApp.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<TokenService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var signingKey = builder.Configuration["Jwt:SigningKey"];
        if (string.IsNullOrWhiteSpace(signingKey) || Encoding.UTF8.GetByteCount(signingKey) < 32)
            throw new InvalidOperationException("Jwt:SigningKey debe contener al menos 32 bytes UTF-8.");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            RequireSignedTokens = true,
            ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 },
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            RequireExpirationTime = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();
var app = builder.Build();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();


app.MapGet("/", () => Results.Ok(new { app = "JwtApp", status = "Autenticación JWT implementada" }));


app.MapPost("/api/auth/token", (LoginRequest request, UserService users, TokenService tokens) =>
{
    if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        return Results.BadRequest(new { message = "Usuario y contraseña son obligatorios." });
    var user = users.ValidateCredentials(request.Username, request.Password);
    if (user is null) return Results.Unauthorized();
    return Results.Ok(tokens.Create(user));
}).AllowAnonymous();


app.MapGet("/api/jwt/profile", (ClaimsPrincipal user) => Results.Ok(new ProfileResponse(
    user.FindFirstValue(ClaimTypes.NameIdentifier) ?? "",
    user.Identity?.Name ?? "",
    user.FindFirstValue(ClaimTypes.Role) ?? "")))
    .RequireAuthorization();
app.Run();
