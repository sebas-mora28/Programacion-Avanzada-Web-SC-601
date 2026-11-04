# Semana 8: Data Annotations y Fluent API

El proyecto de consola [BusquedaBenchmark](BusquedaBenchmark/README.md) compara búsqueda lineal y binaria con BenchmarkDotNet, distintos tamaños de arreglos y valores presentes o ausentes.

El proyecto de consola [GeneradorCsv](GeneradorCsv/README.md) genera archivos CSV usando `DataTable`, `StringBuilder` y `CsvHelper`.

El ejemplo [PruebasDemo](PruebasDemo/README.md) explica pruebas unitarias con xUnit y Moq y pruebas de integración con Microsoft.AspNetCore.Mvc.Testing.

También está disponible el programa de consola [DesayunoAsincrono](DesayunoAsincrono/README.md), que prepara café y tostadas usando `async`, `await` y `Task.WhenAll`.

Dos proyectos independientes de ASP.NET Core y EF Core 10 implementan el mismo catálogo de productos y categorías. En esta práctica, **método tradicional** significa configurar las entidades con **atributos (Data Annotations) y convenciones**. Fluent API configura ese mismo modelo mediante llamadas encadenadas en `OnModelCreating`.

## Ejecutar

Requisitos: .NET SDK 10, la herramienta `dotnet-ef` 10 y PostgreSQL. Se incluye Docker Compose para levantar PostgreSQL 17 con datos persistentes. Ambos proyectos incluyen `Microsoft.EntityFrameworkCore.Design` para ejecutar los comandos de EF Core.

Si no tiene la herramienta instalada:

```bash
dotnet tool install --global dotnet-ef --version 10.0.0
```

Desde la raíz del repositorio:

```bash
docker compose -f semana-8/compose.yaml up -d --wait
dotnet build semana-8/Semana8.slnx
dotnet ef database update --project semana-8/Tradicional
dotnet run --project semana-8/Tradicional
```

En otra terminal:

```bash
dotnet ef database update --project semana-8/FluentApi
dotnet run --project semana-8/FluentApi
```

| Proyecto | Dirección | Configuración que debe revisar |
|---|---|---|
| Tradicional | http://localhost:5081 | [Producto.cs](Tradicional/Models/Producto.cs), [Categoria.cs](Tradicional/Models/Categoria.cs) |
| FluentApi | http://localhost:5082 | [AppDbContext.cs](FluentApi/Data/AppDbContext.cs) |

`dotnet ef database update` crea la base y aplica las migraciones incluidas en cada proyecto: `semana8_tradicional` y `semana8_fluentapi`. Al arrancar, cada aplicación agrega una categoría **Tecnología** con un producto **Teclado** cuando no hay categorías. Los cambios de datos en un proyecto no afectan al otro.

**Si ejecutó una versión anterior con `EnsureCreated`:** esa base no tiene historial de migraciones y la migración inicial intentará crear tablas que ya existen. Para conservar los datos anteriores, configure un nombre de base nuevo en `appsettings.json` antes de ejecutar los comandos. Use la misma conexión para las herramientas y la aplicación. No mezcle `EnsureCreated` con migraciones.

### Conexión a PostgreSQL

El contenedor publica PostgreSQL en `localhost:5433`, para evitar conflictos con el puerto habitual `5432` de otras prácticas. Ambos proyectos usan el proveedor `Npgsql.EntityFrameworkCore.PostgreSQL` y `UseNpgsql`.

Las conexiones están en el `appsettings.json` de cada proyecto:

```text
Host=localhost;Port=5433;Database=semana8_tradicional;Username=postgres;Password=postgres123
Host=localhost;Port=5433;Database=semana8_fluentapi;Username=postgres;Password=postgres123
```

Si ya tiene PostgreSQL instalado, puede omitir Docker y ajustar esos archivos, o sobrescribir la conexión con una variable de entorno:

```bash
ConnectionStrings__DefaultConnection='Host=localhost;Port=5432;Database=semana8_tradicional;Username=postgres;Password=postgres123' \
  dotnet run --project semana-8/Tradicional
```

Para FluentApi, use `Database=semana8_fluentapi` y `--project semana-8/FluentApi`. El usuario debe poder crear las bases de datos, o estas deben existir vacías y permitirle crear sus tablas. Las credenciales incluidas son únicamente para la práctica local.

Para detener PostgreSQL conservando los datos:

```bash
docker compose -f semana-8/compose.yaml down
```

## Comparación del código

Tradicional, en la entidad:

```csharp
[Required, MaxLength(100)]
[Column("nombre_producto")]
public string Nombre { get; set; } = string.Empty;
```

Fluent API, en el contexto:

```csharp
entity.Property(p => p.Nombre)
    .HasColumnName("nombre_producto")
    .IsRequired()
    .HasMaxLength(100);
```

| Configuración | Tradicional | Fluent API |
|---|---|---|
| Tabla | `[Table("Productos")]` | `ToTable("Productos")` |
| Clave primaria | `[Key]` | `HasKey(p => p.Id)` |
| Columna obligatoria | `[Required]` | `IsRequired()` |
| Longitud máxima | `[MaxLength(100)]` | `HasMaxLength(100)` |
| Nombre de columna | `[Column("nombre_producto")]` | `HasColumnName("nombre_producto")` |
| Precisión decimal | `[Precision(10, 2)]` | `HasPrecision(10, 2)` |
| Índice único | `[Index(nameof(Nombre), IsUnique = true)]` | `HasIndex(c => c.Nombre).IsUnique()` |
| Relación uno a muchos | `[ForeignKey(nameof(CategoriaId))]` y navegaciones | `HasOne(...).WithMany(...).HasForeignKey(...)` |
| Borrado en cascada | Convención de relación obligatoria | `OnDelete(DeleteBehavior.Cascade)` explícito |
| Propiedad excluida | `[NotMapped]` | `Ignore(p => p.Etiqueta)` |

`Index` y `Precision` son atributos propios de EF Core. Los demás atributos mostrados pertenecen a `System.ComponentModel.DataAnnotations` o a su espacio `Schema`.

Los atributos permiten leer reglas sencillas junto a la entidad. Fluent API concentra el mapeo fuera de las entidades y permite configuraciones más complejas, como conversiones de valores y filtros globales. Si se combinan mecanismos, la precedencia es **Fluent API > atributos > convenciones**. Consulte la [documentación oficial de configuración de modelos](https://learn.microsoft.com/en-us/ef/core/modeling/).

## Demostración en clase

1. Compare `Models/Producto.cs` y `Models/Categoria.cs` entre proyectos: las entidades de FluentApi no contienen atributos de persistencia.
2. Compare ambos `Data/AppDbContext.cs`: el tradicional no necesita `OnModelCreating`; el otro concentra las reglas allí.
3. Abra `/api/esquema` en ambos puertos. Las tablas, columnas, claves e índices generados deben ser iguales.
4. Use el archivo `pruebas.http` de cada proyecto con REST Client de VS Code, o reproduzca las solicitudes en Postman. Cree, consulte, actualice y elimine un producto.
5. Envíe un nombre vacío o precio negativo: ambos proyectos devuelven `400` porque usan la misma validación HTTP.
6. Como ejercicio, cambie `MaxLength(100)` por `MaxLength(120)` en el proyecto tradicional y `HasMaxLength(100)` por `HasMaxLength(120)` en FluentApi. Observe dónde se realiza cada cambio. Para aceptar nombres más largos por HTTP, cambie también el DTO de cada proyecto.

## Endpoints

| Método | Ruta | Resultado |
|---|---|---|
| GET | `/api/categorias` | Categorías disponibles |
| GET | `/api/productos` | Productos y nombre de su categoría |
| GET | `/api/productos/{id}` | Producto o `404` |
| POST | `/api/productos` | Crea y devuelve `201` con ID y cabecera `Location` |
| PUT | `/api/productos/{id}` | Actualiza y devuelve `204`, o `404` |
| DELETE | `/api/productos/{id}` | Elimina y devuelve `204`, o `404` |
| GET | `/api/esquema` | SQL de creación generado por EF Core |

Ejemplo para crear un producto (use una categoría existente):

```bash
curl -i http://localhost:5081/api/productos \
  -H 'Content-Type: application/json' \
  -d '{"nombre":"Mouse","descripcion":null,"precio":8500,"categoriaId":1}'
```

Repita con el puerto `5082` para FluentApi.

## Mapeo y validación son responsabilidades diferentes

`ProductoRequest` es un DTO independiente con las mismas reglas en ambos proyectos. Los endpoints invocan explícitamente su validación con `Validator.TryValidateObject`; además verifican que exista la categoría. Tener atributos en este DTO no convierte el mapeo de FluentApi en Data Annotations: EF solo mapea `Producto` y `Categoria`.

Fluent API configura la persistencia; no valida automáticamente las solicitudes HTTP. EF Core tampoco valida la longitud máxima ni la precisión antes de enviar valores al proveedor. Consulte la [documentación oficial sobre propiedades](https://learn.microsoft.com/en-us/ef/core/modeling/entity-properties).

PostgreSQL aplica claves, unicidad, `NOT NULL`, longitudes como `character varying(100)` y precisión/escala como `numeric(10,2)`. Estos tipos se pueden observar en `/api/esquema`. La validación del DTO limita la longitud y el rango del precio; si se envían más de dos decimales, PostgreSQL redondea al guardar en `numeric(10,2)`.

## Migraciones

Cada proyecto incluye una migración `Inicial` en `Data/Migrations`. No necesita crearla nuevamente. Desde la carpeta del proyecto (por ejemplo, `semana-8/FluentApi`):

```bash
dotnet ef database update
dotnet run
```

Después de cambiar una entidad o su configuración, genere y revise una nueva migración y luego aplíquela:

```bash
dotnet ef migrations add NombreDelCambio --output-dir Data/Migrations
dotnet ef database update
```

Las migraciones de los dos proyectos son independientes. `Program.cs` agrega datos de ejemplo, pero no crea ni actualiza el esquema automáticamente. Consulte la [documentación de herramientas de EF Core](https://learn.microsoft.com/en-us/ef/core/cli/dotnet) y la [guía para aplicar migraciones](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/applying).
