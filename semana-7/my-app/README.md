# RazorPagesApp

Proyecto ASP.NET Core con Razor Pages para .NET 10.

## Ejecutar

Desde esta carpeta, con el SDK de .NET 10 instalado:

```sh
dotnet run --launch-profile http
```

Abrir http://localhost:5100 en el navegador. Para detener el servidor, presionar Ctrl+C.

## Compilar

```sh
dotnet build
```

## Estructura

- `Pages/`: páginas Razor (`.cshtml`) y sus modelos (`.cshtml.cs`).
- `Pages/Shared/`: diseño compartido.
- `wwwroot/`: archivos CSS, JavaScript y bibliotecas estáticas.
- `Program.cs`: configuración de servicios y rutas.
- `appsettings.json`: configuración de la aplicación.
