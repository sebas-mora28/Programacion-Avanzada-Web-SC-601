using System.ComponentModel.DataAnnotations;

namespace EFCore_Semana6.Models;

public class UsuarioRequest
{
    [Required, MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(150)]
    public string Email { get; set; } = string.Empty;
}
