using Procesamiento.CSV.Services;
Console.WriteLine("Ejercicio 3: procesamiento CSV");
var input = Path.Combine(AppContext.BaseDirectory, "products.csv");
var output = Path.Combine(AppContext.BaseDirectory, "filtered_products.csv");
var service = new ProductCsvService();
// TODO: cuando termine de implementar el servicio, descomente:
// service.ExportProductsAbovePrice(input, output);
// Console.WriteLine($"Archivo generado: {output}");
Console.WriteLine("Complete los TODO del servicio para generar el archivo filtrado.");
