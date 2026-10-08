# Semana 2 - Razor y Model Binding en ASP.NET Core

Proyecto didáctico para demostrar de forma sencilla:

- Razor (`@model`, `@Model`, `@foreach`)
- Model Binding
- Model Class
- Data Annotations
- `ModelState.IsValid`
- Validación personalizada
- `Bind`
- `BindNever`

## Ejecutar

Requiere .NET 10 SDK.

```bash
dotnet restore
dotnet run
```

Luego abra la URL mostrada por `dotnet run` (por defecto el perfil HTTP usa `http://localhost:5187`).

## Flujo del ejemplo

1. Abra **Crear producto**.
2. Envíe un nombre vacío para observar `[Required]`.
3. Use un nombre de 1 o 2 caracteres para observar `[StringLength]`.
4. Use un precio `0`, negativo o mayor que `5000` para observar `PrecioPermitidoAttribute`.
5. Use datos válidos y envíe el formulario.
6. Observe en la pantalla de detalle que `Id` y `FechaCreacion` fueron asignados por el servidor.

## Archivos principales

- `Models/Producto.cs`: clase de modelo, Data Annotations y `BindNever`.
- `Validation/PrecioPermitidoAttribute.cs`: validación personalizada.
- `Controllers/ProductosController.cs`: Model Binding, `Bind` y `ModelState.IsValid`.
- `Views/Productos/Create.cshtml`: formulario Razor, `asp-for` y `asp-validation-for`.
- `Views/Productos/Details.cshtml`: uso de `@model` y `@Model`.
- `Views/Productos/Index.cshtml`: uso de Razor y `@foreach`.

## Nota

El proyecto usa una lista estática en memoria en lugar de una base de datos para que la práctica se concentre exclusivamente en los conceptos de la semana 2.
