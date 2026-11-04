# Generador de CSV

Proyecto de consola para .NET 10 que exporta productos de ejemplo utilizando los tres componentes juntos:

- `DataTable`: define las columnas y almacena las filas de productos en memoria.
- `StringBuilder`: acumula el contenido del CSV mediante un `StringWriter`.
- `CsvHelper`: escribe encabezados y campos, escapando comas, comillas y saltos de línea.

## Ejecutar

Requisito: SDK de .NET 10. La primera ejecución restaura el paquete NuGet `CsvHelper` 33.1.0 y requiere acceso a Internet.

Desde la raíz del repositorio:

```bash
dotnet run --project semana-8/GeneradorCsv -- semana-8/GeneradorCsv/productos.csv
```

O desde esta carpeta:

```bash
dotnet run
```

Sin argumentos, se crea `productos.csv` en el directorio de trabajo actual. Para elegir otra ruta:

```bash
dotnet run -- "salida/reporte de productos.csv"
```

El programa crea las carpetas de destino que falten y sobrescribe el archivo si ya existe. Muestra el CSV y su ruta completa en consola. Devuelve código `0` cuando termina correctamente y `1` si los argumentos o la escritura fallan.

## Datos y formato

Edite `CrearProductos()` en `Program.cs` para cambiar las columnas o los datos. `GenerarCsv(DataTable tabla)` recorre las columnas y filas de la tabla recibida.

El CSV incluye encabezados, usa coma como separador y punto decimal (`CultureInfo.InvariantCulture`). Se guarda en UTF-8 con BOM para conservar las tildes en Excel. Si Excel no separa las columnas según su configuración regional, impórtelo desde **Datos → Desde texto/CSV** y seleccione coma como delimitador.

Los ejemplos incluyen tildes, una coma dentro de un nombre, comillas, una descripción con salto de línea y un valor `DBNull`, que se exporta como campo vacío. El contenido completo se construye en memoria para demostrar `StringBuilder`.

Referencias: [CsvHelper: uso y escritura de campos](https://joshclose.github.io/CsvHelper/getting-started/) y [paquete NuGet](https://www.nuget.org/packages/CsvHelper/33.1.0).
