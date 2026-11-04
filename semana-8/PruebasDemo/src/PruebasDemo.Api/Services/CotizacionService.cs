using PruebasDemo.Api.Models;
using PruebasDemo.Api.Repositories;

namespace PruebasDemo.Api.Services;

public class CotizacionService(IProductoRepository repository)
{
    public async Task<Cotizacion?> CotizarAsync(
        int productoId, int cantidad, CancellationToken cancellationToken = default)
    {
        // Regla del negocio: también se protege cuando el servicio se usa fuera de HTTP.
        // Las pruebas unitarias comprueban que una cantidad inválida ni siquiera consulte el repositorio.
        if (cantidad is < 1 or > 100)
            throw new ArgumentOutOfRangeException(nameof(cantidad), "La cantidad debe estar entre 1 y 100.");

        var producto = await repository.ObtenerPorIdAsync(productoId, cancellationToken);
        if (producto is null) return null;

        decimal subtotal = producto.Precio * cantidad;
        // A partir de cinco unidades, el descuento es del 10 % del subtotal.
        // Redondeamos el descuento a dos decimales antes de restarlo.
        decimal descuento = cantidad >= 5
            ? decimal.Round(subtotal * 0.10m, 2, MidpointRounding.AwayFromZero)
            : 0m;

        return new Cotizacion(producto.Id, producto.Nombre, cantidad, producto.Precio,
            subtotal, descuento, subtotal - descuento);
    }
}
