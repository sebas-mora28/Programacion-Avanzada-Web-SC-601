using MvcProductosClase.Models;

namespace MvcProductosClase.Services;

// Datos en memoria: se reinician cuando se detiene la aplicación.
public class ProductoRepository
{
    private readonly List<Producto> _productos =
    [
        new() { Id = 1, Nombre = "Teclado", Precio = 25000 },
        new() { Id = 2, Nombre = "Mouse", Precio = 12000 }
    ];
    private int _siguienteId = 3;
    private readonly object _sync = new();

    public List<Producto> Todos()
    {
        lock (_sync) return _productos.Select(Copiar).ToList();
    }

    public Producto? Buscar(int id)
    {
        lock (_sync) return _productos.Where(p => p.Id == id).Select(Copiar).FirstOrDefault();
    }

    public void Crear(Producto producto)
    {
        lock (_sync)
        {
            var copia = Copiar(producto);
            copia.Id = _siguienteId++;
            _productos.Add(copia);
        }
    }

    public bool Actualizar(int id, Producto producto)
    {
        lock (_sync)
        {
            var actual = _productos.FirstOrDefault(p => p.Id == id);
            if (actual is null) return false;
            actual.Nombre = producto.Nombre;
            actual.Precio = producto.Precio;
            return true;
        }
    }

    public bool Eliminar(int id)
    {
        lock (_sync)
        {
            var actual = _productos.FirstOrDefault(p => p.Id == id);
            return actual is not null && _productos.Remove(actual);
        }
    }

    private static Producto Copiar(Producto p) => new() { Id = p.Id, Nombre = p.Nombre, Precio = p.Precio };
}
