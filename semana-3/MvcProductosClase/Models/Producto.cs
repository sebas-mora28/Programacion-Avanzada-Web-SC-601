using System.ComponentModel.DataAnnotations;

namespace MvcProductosClase.Models;

public class Producto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(80, ErrorMessage = "Máximo 80 caracteres.")]
    public string Nombre { get; set; } = "";

    [Range(0.01, 10000000, ErrorMessage = "El precio debe ser mayor que cero.")]
    public decimal Precio { get; set; }
}
