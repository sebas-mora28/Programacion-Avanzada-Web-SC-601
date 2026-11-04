using Procesamiento.CSV.Models;
namespace Procesamiento.CSV.Services;
public class ProductCsvService
{
    public IReadOnlyList<Product> ReadProducts(string inputPath)
    {
        // TODO: leer el archivo usando CsvHelper y convertir las filas en Product.
        throw new NotImplementedException();
    }

    public void ExportProductsAbovePrice(string inputPath, string outputPath, decimal minimumPrice = 30m)
    {
        // TODO: obtener productos, filtrar Price > minimumPrice,
        // y escribir filtered_products.csv con encabezado Id,Name,Price.
        throw new NotImplementedException();
    }
}
