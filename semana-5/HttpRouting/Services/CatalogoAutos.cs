using System.Collections.Concurrent;
using Semana05HttpRouting.Models;

namespace Semana05HttpRouting.Services;

public class CatalogoAutos
{
    private readonly ConcurrentDictionary<int, Auto> _autos = new();
    private int _ultimoId = 3;

    public CatalogoAutos()
    {
        _autos[1] = new Auto(1, "Toyota", "Corolla", 9500000m);
        _autos[2] = new Auto(2, "Hyundai", "Accent", 7200000m);
        _autos[3] = new Auto(3, "Honda", "Civic", 11000000m);
    }

    public Auto[] Buscar(string? marca, decimal? precioMaximo) => _autos.Values
        .Where(a => string.IsNullOrWhiteSpace(marca) ||
            a.Marca.Equals(marca.Trim(), StringComparison.OrdinalIgnoreCase))
        .Where(a => precioMaximo is null || a.Precio <= precioMaximo.Value)
        .OrderBy(a => a.Id)
        .ToArray();

    public Auto? Obtener(int id) => _autos.TryGetValue(id, out var auto) ? auto : null;

    public Auto Crear(CrearAuto entrada)
    {
        // El endpoint valida antes de invocar este método.
        var id = Interlocked.Increment(ref _ultimoId);
        var auto = new Auto(id, entrada.Marca!.Trim(), entrada.Modelo!.Trim(), entrada.Precio);
        _autos[id] = auto;
        return auto;
    }
}
