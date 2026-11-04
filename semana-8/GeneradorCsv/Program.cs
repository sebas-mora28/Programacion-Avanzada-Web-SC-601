using System.Data;
using System.Globalization;
using System.Text;
using CsvHelper;

if (args.Length > 1)
{
    Console.Error.WriteLine("Uso: dotnet run -- [ruta-del-archivo.csv]");
    return 1;
}

try
{
    using DataTable productos = CrearProductos();
    string contenido = GenerarCsv(productos);
    string ruta = Path.GetFullPath(args.Length == 1 ? args[0] : "productos.csv");

    Directory.CreateDirectory(Path.GetDirectoryName(ruta)!);
    // UTF-8 con BOM permite reconocer las tildes al abrir el archivo en Excel.
    File.WriteAllText(ruta, contenido, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));

    Console.WriteLine(contenido);
    Console.WriteLine($"CSV generado con {productos.Rows.Count} productos: {ruta}");
    return 0;
}
catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
{
    Console.Error.WriteLine($"No se pudo generar el CSV: {ex.Message}");
    return 1;
}

static DataTable CrearProductos()
{
    // DataTable almacena datos en memoria con columnas y tipos definidos.
    var tabla = new DataTable("Productos");
    tabla.Columns.Add("Id", typeof(int));
    tabla.Columns.Add("Nombre", typeof(string));
    tabla.Columns.Add("Precio", typeof(decimal));
    tabla.Columns.Add("Existencias", typeof(int));
    tabla.Columns.Add("Descripcion", typeof(string));

    tabla.Rows.Add(1, "Café molido", 3500.50m, 20, "Bolsa de 500 g");
    tabla.Rows.Add(2, "Teclado, español", 15000.00m, 8, "Distribución en español");
    tabla.Rows.Add(3, "Monitor de 24\"", 95000.75m, 5, "Pantalla Full HD");
    tabla.Rows.Add(4, "Mouse inalámbrico", 8500.00m, 15, DBNull.Value);
    tabla.Rows.Add(5, "Cuaderno", 1200.00m, 30, "Tapa dura\n80 hojas");
    return tabla;
}

static string GenerarCsv(DataTable tabla)
{
    // StringBuilder acumula el CSV; StringWriter lo conecta con CsvHelper.
    var contenido = new StringBuilder();
    using var escritor = new StringWriter(contenido, CultureInfo.InvariantCulture);
    using var csv = new CsvWriter(escritor, CultureInfo.InvariantCulture);

    foreach (DataColumn columna in tabla.Columns)
    {
        csv.WriteField(columna.ColumnName);
    }
    csv.NextRecord();

    foreach (DataRow fila in tabla.Rows)
    {
        foreach (DataColumn columna in tabla.Columns)
        {
            // CsvHelper escapa comas, comillas y saltos de línea.
            // La cultura invariante usa punto decimal y coma como separador.
            string valor = fila.IsNull(columna)
                ? string.Empty
                : Convert.ToString(fila[columna], CultureInfo.InvariantCulture) ?? string.Empty;
            csv.WriteField(valor);
        }
        csv.NextRecord();
    }

    // Vaciar el búfer antes de obtener el contenido del StringBuilder.
    csv.Flush();
    return contenido.ToString();
}
