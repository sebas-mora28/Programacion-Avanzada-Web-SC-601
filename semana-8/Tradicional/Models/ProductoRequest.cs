using System.ComponentModel.DataAnnotations;

namespace Tradicional.Models;

// Estos atributos validan la entrada HTTP; NO configuran el modelo de EF.
// El DTO y la validación son iguales en ambos proyectos.
public class ProductoRequest
{
    [Required, MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? Descripcion { get; set; }

    [Range(typeof(decimal), "0.01", "99999999.99")]
    public decimal Precio { get; set; }

    [Range(1, int.MaxValue)]
    public int CategoriaId { get; set; }

    public Dictionary<string, string[]> Validar()
    {
        var resultados = new List<ValidationResult>();
        Validator.TryValidateObject(this, new ValidationContext(this), resultados, true);
        return resultados
            .SelectMany(r => r.MemberNames.Select(nombre => new { nombre, r.ErrorMessage }))
            .GroupBy(r => r.nombre)
            .ToDictionary(g => g.Key, g => g.Select(r => r.ErrorMessage!).ToArray());
    }
}
