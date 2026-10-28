using Microsoft.AspNetCore.Identity;

namespace AuthLab;

public sealed record AppUser(string Id, string Username, string Role, string PasswordHash);
public sealed record LoginRequest(string? Username, string? Password);
public sealed record ProfileResponse(string Id, string Username, string Role);
public sealed record TokenResponse(string AccessToken, string TokenType, int ExpiresIn);

// Infraestructura proporcionada. No modificar ni devolver PasswordHash al cliente.
public sealed class UserService
{
    private readonly AppUser[] _users =
    [
        new("1", "admin", "Admin", "passwordAdmin"),
        new("2", "estudiante", "User", "password123"),
    ];

    public AppUser? ValidateCredentials(string? username, string? password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password)) return null;
        var user = _users.FirstOrDefault(u =>
            string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase));
        if (user is null) return null;
        if(user.PasswordHash == password)
        {
            return user;
        }
        return null;
    }
}
