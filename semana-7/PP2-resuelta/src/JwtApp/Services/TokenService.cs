using AuthLab;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JwtApp.Services;

public sealed class TokenService(IConfiguration configuration)
{
    public TokenResponse Create(AppUser user)
    {
        var issuer = configuration["Jwt:Issuer"];
        var audience = configuration["Jwt:Audience"];
        var signingKey = configuration["Jwt:SigningKey"];
        var expirationMinutes = configuration.GetValue<int>("Jwt:ExpirationMinutes");
        if (string.IsNullOrWhiteSpace(signingKey) || Encoding.UTF8.GetByteCount(signingKey) < 32)
            throw new InvalidOperationException("Jwt:SigningKey debe contener al menos 32 bytes UTF-8.");
        if (string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audience)
            || expirationMinutes <= 0)
            throw new InvalidOperationException("Configure Jwt:Issuer, Jwt:Audience y Jwt:ExpirationMinutes positivo.");

        var now = DateTime.UtcNow;
        var expiresIn = checked(expirationMinutes * 60);
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim("role", user.Role)
            },
            notBefore: now,
            expires: now.AddSeconds(expiresIn),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                SecurityAlgorithms.HmacSha256));
        return new TokenResponse(new JwtSecurityTokenHandler().WriteToken(token), "Bearer", expiresIn);
    }
}
