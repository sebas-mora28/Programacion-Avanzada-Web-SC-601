# Semana 6 — Entity Framework Core y API

Proyecto académico de **ASP.NET Core Minimal API**, **EF Core 10** y **PostgreSQL**. El gestor de tareas incluye relaciones entre tareas, categorías y usuarios.

## Requisitos

- .NET SDK 10
- Docker Desktop (o PostgreSQL configurado localmente)
- `dotnet-ef` 10 (`dotnet tool install --global dotnet-ef --version 10.0.0`)

## Ejecutar

Ejecute los comandos desde la carpeta que contiene `EFCore_Semana6.csproj`. Primero inicie PostgreSQL con los comandos de la sección [PostgreSQL con Docker](#postgresql-con-docker), o utilice una instancia existente accesible con la cadena de conexión de `appsettings.json`.

```bash
dotnet restore
dotnet ef database update
dotnet run --urls http://localhost:5080
```

La primera inicialización de PostgreSQL puede tardar unos segundos. Puede comprobar su disponibilidad con `docker exec tareas-postgres pg_isready -U postgres -d TareasDb`. Si la migración falla por conexión, espere y repita `dotnet ef database update`.

Las migraciones incluidas crean las tablas, las relaciones, la categoría `General`, la función de consulta y el procedimiento de reasignación. La aplicación no aplica migraciones automáticamente al iniciar.

En VS Code, use **pruebas.http** con la extensión REST Client o importe [la colección completa de Postman](postman/Tareas.postman_collection.json), que incluye tareas, categorías, usuarios y reasignaciones. Ajuste las variables de IDs para que coincidan con registros existentes.

## Opciones de base de datos

El proyecto está configurado actualmente para **PostgreSQL**. Los comandos de SQL Server y SQLite son alternativas de configuración; requieren adaptar el proyecto antes de ejecutar sus migraciones y endpoints SQL. Los comandos multilínea usan sintaxis Bash/Zsh.

### PostgreSQL con Docker

Crear el volumen y el contenedor:

```bash
docker volume create tareas_postgres_data

docker run --name tareas-postgres \
  --detach \
  --env POSTGRES_USER=postgres \
  --env POSTGRES_PASSWORD=postgres123 \
  --env POSTGRES_DB=TareasDb \
  --publish 5432:5432 \
  --volume tareas_postgres_data:/var/lib/postgresql/data \
  postgres:17
```

Cadena de conexión:

```text
Host=localhost;Port=5432;Database=TareasDb;Username=postgres;Password=postgres123
```

Proveedor de EF Core (ya incluido en este proyecto):

```bash
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 10.0.0
```

```csharp
options.UseNpgsql(connectionString);
```

Para volver a iniciar el mismo contenedor:

```bash
docker start tareas-postgres
```

### SQL Server con Docker

Crear el volumen y el contenedor:

```bash
docker volume create tareas_sqlserver_data

docker run --name tareas-sqlserver \
  --detach \
  --env ACCEPT_EULA=Y \
  --env 'MSSQL_SA_PASSWORD=SqlServer123!' \
  --publish 1433:1433 \
  --volume tareas_sqlserver_data:/var/opt/mssql \
  mcr.microsoft.com/mssql/server:2022-latest
```

SQL Server usa imágenes x86-64. En Apple Silicon, agregar `--platform linux/amd64` requiere emulación y no garantiza su funcionamiento: Microsoft no prueba ni soporta estos entornos. Consulte la [documentación de SQL Server en contenedores](https://learn.microsoft.com/en-us/sql/linux/containers/deploy?view=sql-server-linux-ver15).

Cadena de conexión:

```text
Server=localhost,1433;Database=TareasDb;User Id=sa;Password=SqlServer123!;TrustServerCertificate=True
```

Proveedor de EF Core:

```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 10.0.0
```

```csharp
options.UseSqlServer(connectionString);
```

Para volver a iniciar el mismo contenedor:

```bash
docker start tareas-sqlserver
```

### SQLite

SQLite no necesita un servidor ni un contenedor. La base de datos es un archivo local.

Instalar el proveedor:

```bash
dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 10.0.0
```

Cadena de conexión:

```text
Data Source=tareas.db
```

Configuración de EF Core:

```csharp
options.UseSqlite(connectionString);
```

Después de adaptar el proyecto y generar migraciones para SQLite, crear o actualizar el archivo de base de datos mediante EF Core:

```bash
dotnet ef database update
```

Si está instalada la herramienta de línea de comandos de SQLite, puede abrirse con:

```bash
sqlite3 tareas.db
```

### Importante al cambiar de proveedor

- Debe instalar y configurar solamente el proveedor que vaya a utilizar.
- Cambie la cadena `ConnectionStrings:DefaultConnection` en `appsettings.json` y la llamada al proveedor tanto en `Program.cs` como en `Data/AppDbContextFactory.cs` (usada por las herramientas de migración).
- Las migraciones suelen ser específicas para cada motor; genere migraciones independientes al cambiar de proveedor.
- La función y el procedimiento almacenado incluidos en este proyecto usan sintaxis de PostgreSQL y no funcionan directamente en SQL Server o SQLite.
- La llamada con `NpgsqlParameter` y `CALL` también requiere adaptación al proveedor elegido; cambiar solamente la cadena de conexión no es suficiente.
- SQLite no soporta procedimientos almacenados.

## Temas y ubicación

| Tema | Archivo |
|---|---|
| Entidades, solicitudes y proyección de detalle | `Models/` |
| DbContext, DbSet, llaves foráneas e índices | `Data/AppDbContext.cs` |
| Configuración para herramientas de migración | `Data/AppDbContextFactory.cs` |
| Cadena de conexión | `appsettings.json` y `Program.cs` |
| Migraciones, función SQL y procedimiento almacenado | `Data/Migrations/` |
| Endpoints Minimal API, CRUD y SQL ejecutado desde EF | `Endpoints/TareasEndpoints.cs` |
| Operaciones de categorías y usuarios | `Endpoints/CategoriasEndpoints.cs`, `Endpoints/UsuariosEndpoints.cs` |
| Reasignación con EF Core o procedimiento almacenado | `Endpoints/ReasignacionesEndpoints.cs` |
| Validación de solicitudes con DataAnnotations | `Endpoints/RequestValidator.cs` |
| Pruebas HTTP | `pruebas.http` |

## Modelo relacional

- Una `Categoria` tiene muchas `Tareas`; cada tarea requiere una categoría.
- Un `Usuario` puede tener muchas tareas asignadas; una tarea puede no tener responsable.
- La categoría usa borrado restringido para proteger sus tareas.
- Al eliminar un usuario, `ResponsableId` pasa a `NULL` mediante `ON DELETE SET NULL`.
- `Categoria.Nombre` y `Usuario.Email` tienen índices únicos.
- Las solicitudes de tareas validan título (obligatorio, hasta 100 caracteres), descripción (hasta 500), prioridad (obligatoria, hasta 20) y existencia de la categoría y del responsable indicado. `Prioridad` tiene valor inicial `Media`; no es una enumeración cerrada.

## Endpoints

- `GET /api/tareas`
- `GET /api/tareas/{id}`
- `POST /api/tareas`
- `PUT /api/tareas/{id}`
- `DELETE /api/tareas/{id}`
- `GET /api/tareas/por-estado?completada=true` (función de PostgreSQL)
- `GET /api/tareas/detalle` (`INNER JOIN` con categorías y `LEFT JOIN` con usuarios)
- `GET /api/tareas/resumen-por-categoria` (agrupación y conteos)
- `POST /api/tareas/reasignar-con-transaccion` (UPDATE con EF Core, `CommitAsync` y `RollbackAsync`)
- `POST /api/tareas/reasignar-con-procedimiento` (solamente ejecuta `CALL reasignar_tareas`)
- `GET/POST /api/categorias`
- `PUT/DELETE /api/categorias/{id}`
- `GET/POST /api/usuarios`
- `PUT/DELETE /api/usuarios/{id}`

La ruta `/` redirige a `/api/tareas`. Categorías y usuarios tienen consulta de listado; actualmente no hay un `GET /api/categorias/{id}` ni un `GET /api/usuarios/{id}`.

`detalle` proyecta el resultado en `TareaDetalle`: incluye el nombre de la categoría y el nombre/correo del responsable. El `LEFT JOIN` conserva las tareas sin responsable. `resumen-por-categoria` calcula totales, completadas y pendientes, incluyendo categorías sin tareas.

## Función y procedimiento almacenado

La migración `RelacionesCategoriasUsuarios` crea una categoría `General` y la asigna a las tareas existentes antes de aplicar la llave foránea obligatoria.

La función `obtener_tareas_por_estado(boolean)` se crea en la migración inicial y se actualiza en `RelacionesCategoriasUsuarios` para devolver las nuevas columnas. `GET /api/tareas/por-estado` la consulta mediante `FromSqlInterpolated`.

El procedimiento `reasignar_tareas(integer, integer)` se define con SQL escrito manualmente en `Up()` de [ProcedimientoReasignarTareas](Data/Migrations/20261014054528_ProcedimientoReasignarTareas.cs), mediante `migrationBuilder.Sql(...)`. Su método `Down()` elimina el procedimiento. Se crea al aplicar las migraciones:

```bash
dotnet ef database update
```

El procedimiento valida que ambos usuarios existan y sean diferentes, y ejecuta un `UPDATE` sobre todas las tareas del usuario de origen, incluidas las completadas. No se limita a las pendientes.

### Dos formas de reasignar las mismas tareas

Ambos endpoints están en [ReasignacionesEndpoints.cs](Endpoints/ReasignacionesEndpoints.cs) y reciben el mismo cuerpo:

```json
{
  "usuarioOrigenId": 1,
  "usuarioDestinoId": 2
}
```

Use los identificadores reales de dos usuarios existentes.

| Endpoint (POST) | Ejecución | Control de transacción |
|---|---|---|
| `/api/tareas/reasignar-con-transaccion` | Carga tareas con LINQ, modifica `ResponsableId` y llama a `SaveChangesAsync` | `BeginTransactionAsync`, `CommitAsync` y `RollbackAsync` |
| `/api/tareas/reasignar-con-procedimiento` | Ejecuta `CALL reasignar_tareas(...)` con dos `NpgsqlParameter` | Sin transacción explícita en C# |

El primer endpoint valida la solicitud antes de abrir la transacción. Devuelve `400` por validación, `200` con el número de tareas reasignadas si termina correctamente, o `500` después de revertir si ocurre una excepción dentro del bloque transaccional. No llama al procedimiento almacenado.

El segundo endpoint ejecuta el procedimiento mediante `ExecuteSqlRawAsync` y devuelve `200` con un mensaje de éxito. Las validaciones de usuarios se realizan dentro del procedimiento; sus excepciones no se convierten en respuestas `400` en este endpoint. No devuelve un conteo de tareas.

`NpgsqlParameter` envía valores tipados al proveedor PostgreSQL sin concatenarlos en el SQL. El procedimiento no incluye `COMMIT` ni `ROLLBACK` internos.

## Secuencia de prueba

Use [pruebas.http](pruebas.http) con la API iniciada:

1. Consulte las categorías o cree una con `POST /api/categorias` y un cuerpo como `{"nombre":"Universidad"}`. Anote su ID.
2. Cree dos usuarios con `POST /api/usuarios`, usando nombres y correos distintos, por ejemplo `{"nombre":"Ana","email":"ana@example.com"}` y `{"nombre":"Luis","email":"luis@example.com"}`. El archivo HTTP incluye una solicitud de creación; repítala con los datos del segundo usuario.
3. Cree una tarea usando la categoría y el usuario de origen. Ajuste los IDs de los ejemplos; no asuma que siempre serán `1` y `2`.
4. Consulte `/api/tareas/detalle`, `/api/tareas/resumen-por-categoria` y `/api/tareas/por-estado?completada=false`.
5. Ejecute la reasignación con transacción del primer usuario al segundo y compruebe el cambio en `detalle`.
6. Para comparar el procedimiento sobre esas mismas tareas, invierta los IDs: reasigne del segundo usuario al primero con `/api/tareas/reasignar-con-procedimiento`.

Ejemplo de cuerpo para crear o actualizar una tarea:

```json
{
  "titulo": "Preparar la clase de EF Core",
  "descripcion": "Practicar relaciones, joins y transacciones",
  "completada": false,
  "prioridad": "Alta",
  "categoriaId": 1,
  "responsableId": 1
}
```

Use `"responsableId": null` para dejar una tarea sin responsable. El borrado de una categoría con tareas devuelve `409`; el borrado de un usuario deja sus tareas sin responsable.

## Migraciones siguientes

1. Modifique una entidad o su configuración en `AppDbContext`.
2. Ejecute `dotnet ef migrations add NombreDelCambio --output-dir Data/Migrations`.
3. Revise el código generado en `Data/Migrations`.
4. Ejecute `dotnet ef database update`.

**Nota:** Si agrega columnas a `Tarea`, actualice también el resultado de la función `obtener_tareas_por_estado`. `FromSql` con una entidad requiere recuperar todas sus columnas mapeadas.

Para cambiar el procedimiento, agregue una nueva migración y escriba el SQL en `Up()` y la reversión correspondiente en `Down()`. EF Core no detecta automáticamente cambios en su definición SQL. Modificar una migración ya aplicada no vuelve a ejecutarla.

## Seguridad

La contraseña incluida es **solo para desarrollo local**. En un proyecto real use *User Secrets* o variables de entorno, no registre credenciales en Git. No exponga el puerto de PostgreSQL a redes públicas.

## Notas técnicas

- `DbContext` no equivale necesariamente a una conexión física abierta de forma permanente; administra una unidad de trabajo y EF puede abrir/cerrar conexiones según las operaciones.
- La función de PostgreSQL devuelve todas las columnas de `Tarea` para que EF pueda materializar la entidad. La consulta SQL se ejecuta con un parámetro y luego EF materializa el resultado.
