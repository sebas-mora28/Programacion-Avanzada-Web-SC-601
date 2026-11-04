using System.ComponentModel.DataAnnotations;

namespace PruebasDemo.Api.Models;

public class CotizacionRequest
{
    [Range(1, int.MaxValue)]
    public int ProductoId { get; set; }

    [Range(1, 100)]
    public int Cantidad { get; set; }
}
