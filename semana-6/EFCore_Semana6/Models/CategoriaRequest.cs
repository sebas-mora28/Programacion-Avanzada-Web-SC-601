using System.ComponentModel.DataAnnotations;

namespace EFCore_Semana6.Models;

public class CategoriaRequest
{
    [Required, MaxLength(80)]
    public string Nombre { get; set; } = string.Empty;
}
