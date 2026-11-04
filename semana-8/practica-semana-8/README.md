# Práctica: Fluent API, Unit Tests con Moq y CSV

Una **única solución** `Practica_FluentAPI_Moq_CSV.sln` contiene **tres proyectos independientes**. Cada proyecto tiene su propio `.csproj` y no depende de los otros.

| Proyecto | Carpeta | Objetivo |
|---|---|---|
| FluentAPI.Relaciones | `01_FluentAPI_Relaciones/` | Configurar relaciones 1:N y N:M con EF Core y SQLite. |
| UnitTests.Moq | `02_UnitTests_Moq/` | Implementar `OrderService` y pruebas con xUnit/Moq. |
| Procesamiento.CSV | `03_Procesamiento_CSV/` | Leer y filtrar CSV con CsvHelper y generar otro CSV. |

## Cómo abrir

Abra **`Practica_FluentAPI_Moq_CSV.sln`** desde Visual Studio 2022, o use la terminal (SDK .NET 8 instalado):

```bash
dotnet restore Practica_FluentAPI_Moq_CSV.sln
dotnet build Practica_FluentAPI_Moq_CSV.sln
dotnet test 02_UnitTests_Moq/UnitTests.Moq.csproj
dotnet run --project 03_Procesamiento_CSV/Procesamiento.CSV.csproj
```

La práctica no está resuelta: encontrará comentarios `TODO` en los métodos y configuraciones pendientes. Las pruebas iniciales están preparadas para ser completadas por los estudiantes. Consulte los archivos `README.md` individuales de cada proyecto.

**Nota:** La solución agrupa los tres proyectos, pero no se requieren referencias entre ellos. Para generar migraciones, siga el README del proyecto Fluent API.
