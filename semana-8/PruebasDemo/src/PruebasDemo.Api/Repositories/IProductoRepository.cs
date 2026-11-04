using PruebasDemo.Api.Models;

namespace PruebasDemo.Api.Repositories;

// El servicio depende del contrato, no del lugar donde se guardan los productos.
public interface IProductoRepository
{
    Task<Producto?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
}
