# Practica: Reserva de Citas

Proyecto base para una practica de aproximadamente 60 minutos sobre Razor y Model Binding en ASP.NET Core MVC.

## Requisitos

- .NET 10 SDK
- Visual Studio 2022/2026, Visual Studio Code o Rider

## Ejecutar

```bash
dotnet restore
dotnet run
```

La ruta inicial es `Citas/Crear`.

## Importante

El proyecto esta intencionalmente incompleto. Busque los comentarios `TODO` en:

- `Models/Cita.cs`
- `Validations/FechaFuturaAttribute.cs`
- `Controllers/CitasController.cs`
- `Views/Citas/Crear.cshtml`
- `Views/Citas/Confirmacion.cshtml`

No se requiere base de datos.

## Objetivo

Completar la aplicacion para practicar:

- Razor
- Tag Helpers (`asp-for`, `asp-validation-for`, `asp-validation-summary`)
- Model Binding
- Data Annotations
- Validacion personalizada
- `ModelState.IsValid`
- `BindNever`
- Diferencia entre datos enviados por el cliente y datos controlados por el servidor
