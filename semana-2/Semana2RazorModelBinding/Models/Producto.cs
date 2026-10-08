using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Semana2RazorModelBinding.Validation;

namespace Semana2RazorModelBinding.Models;

public class Producto
{
    // BindNever evita que un valor enviado por el cliente modifique esta propiedad.
    [BindNever]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(60, MinimumLength = 3,
        ErrorMessage = "El nombre debe tener entre 3 y 60 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    // Validación personalizada creada para este ejemplo.
    [PrecioPermitido(1, 5000)]
    public decimal Precio { get; set; }

    // Otra propiedad excluida del Model Binding.
    [BindNever]
    public DateTime FechaCreacion { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Nombre == "1" && Precio < 100)
        {
            yield return new ValidationResult(
                "El producto 'ProductoX' debe tener un precio mayor a 100",
                new[] { nameof(Nombre), nameof(Precio) });
        }
    }
}
