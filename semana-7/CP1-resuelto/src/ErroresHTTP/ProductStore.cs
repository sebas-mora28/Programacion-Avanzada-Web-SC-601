namespace Practica.DisenoHttp;

// Almacenamiento en memoria: los datos se reinician al reiniciar la aplicación.
public sealed class ProductStore
{
    private readonly object _sync = new();
    private readonly Dictionary<int, Product> _products = new()
    {
        [1] = new(1, "Keyboard", 25m),
        [2] = new(2, "Mouse", 15m),
        [3] = new(3, "Monitor", 180m)
    };
    private int _nextId = 4;

    public IReadOnlyList<Product> GetAll()
    {
        lock (_sync)
            return _products.Values.OrderBy(p => p.Id).ToArray();
    }

    public Product? GetById(int id)
    {
        lock (_sync)
            return _products.GetValueOrDefault(id);
    }

    public Product Create(string name, decimal price)
    {
        lock (_sync)
        {
            var product = new Product(_nextId++, name, price);
            _products.Add(product.Id, product);
            return product;
        }
    }

    // Comportamiento heredado del sistema: revisar su semántica.
    public Product? IncreasePriceAndRename(int id, string name, decimal amount)
    {
        lock (_sync)
        {
            if (!_products.TryGetValue(id, out var current)) return null;
            var updated = current with { Name = name, Price = current.Price + amount };
            _products[id] = updated;
            return updated;
        }
    }

    public Product? Replace(int id, string name, decimal price)
    {
        lock (_sync)
        {
            if (!_products.ContainsKey(id)) return null;
            var updated = new Product(id, name, price);
            _products[id] = updated;
            return updated;
        }
    }

    public bool Delete(int id)
    {
        lock (_sync)
            return _products.Remove(id);
    }
}
