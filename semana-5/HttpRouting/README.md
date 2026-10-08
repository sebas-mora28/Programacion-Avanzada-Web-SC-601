# Semana 5 · HTTP y enrutamiento en ASP.NET Core

Proyecto docente basado en **SC-701 Semana 05.pdf**. Incluye 33 ejemplos interactivos, 35 solicitudes en `Ejemplos.http` y el caso integrado **Autos el campeón**.

## Requisitos y ejecución

- **SDK de .NET 10**, no solamente el runtime. Compruebe con `dotnet --list-sdks`.
- Visual Studio, Visual Studio Code o una terminal.
- No necesita base de datos, paquetes NuGet externos ni credenciales.

Abra una terminal en la carpeta que contiene `Semana05HttpRouting.csproj` y ejecute:

```bash
dotnet run
```

Abra **http://localhost:5000/**. Si el navegador no se abre automáticamente, copie la dirección. Detenga el servidor con `Ctrl+C`.

Si el puerto 5000 está ocupado:

```bash
dotnet run --no-launch-profile --urls http://localhost:5050
```

Abra la nueva dirección y cambie `@baseUrl` en `Ejemplos.http`. La interfaz usa rutas relativas y no requiere ajustes. El perfil local usa HTTP de forma intencional; un despliegue real necesita configurar HTTPS.

## Cómo usar el laboratorio

1. Seleccione un tema y un ejemplo en la página inicial.
2. Lea la explicación, el resultado esperado y el archivo de código asociado.
3. Revise método, ruta, encabezados y cuerpo; luego pulse **Enviar solicitud**.
4. Observe estado HTTP, encabezados y contenido de la respuesta.
5. Modifique los datos y vuelva a enviar. Los POST de creación insertan un registro en cada ejecución.

Las solicitudes de `Ejemplos.http` se pueden ejecutar desde un editor compatible con archivos HTTP, como Visual Studio o VS Code con una extensión para ello. También se pueden reproducir manualmente en Postman. Para un POST seleccione cuerpo raw JSON y `Content-Type: application/json`.

## Organización y correspondencia con la presentación

| Archivo | Tema | Diapositivas |
|---|---|---|
| `Program.cs` | Inicio, servicios, registro de endpoints y publicación de archivos | 4 y 10 |
| `Endpoints/GetEndpoints.cs` | Texto, JSON, hora y respuesta sin cuerpo con MapGet | 5 |
| `Endpoints/PostEndpoints.cs` | Cuerpo JSON, validación y cálculo con MapPost | 6 |
| `Endpoints/ParametrosEndpoints.cs` | Ruta, consulta, valores opcionales y encabezados | 7 y 8 |
| `Endpoints/RestriccionesEndpoints.cs` | int, guid, min, length, selección y precedencia | 9 |
| `Endpoints/AutosEndpoints.cs` | Consulta, filtros y creación del catálogo | 11 |
| `Models/Entradas.cs` | DTO de entrada y representación del auto | Apoyo |
| `Services/CatalogoAutos.cs` | Datos en memoria e inyección de dependencias | Apoyo |
| `wwwroot/` | HTML, CSS, JavaScript, SVG y texto públicos | 10 |
| `Ejemplos.http` | Solicitudes listas para ejecutar | Todos |
| `GUIA_CLASE.md` | Recorrido docente, preguntas y ejercicios | Todos |

Los métodos `MapEjemplosGet`, `MapAutos`, etc. son extensiones definidas en este proyecto. No vienen incorporados en ASP.NET Core. Se separaron los archivos por tema para facilitar la exposición. Las capturas de la presentación que usan `IEndpoint` y `MapEndpoint` representan otra forma de organizar el código; aquí no hace falta crear esa interfaz.

## Conceptos que demuestra el código

- **MapGet y MapPost registran handlers**; no envían solicitudes a otras APIs.
- La combinación de **ruta y método HTTP** distingue operaciones. `/api/autos` tiene GET y POST.
- **Binding** convierte valores HTTP a tipos C#. La consulta `pagina=abc` no se convierte a `int`.
- Una **restricción** selecciona rutas; una **validación** aplica reglas y puede devolver un mensaje útil.
- **DTO** delimita los datos de entrada. El cliente no decide el identificador del nuevo auto.
- **DI** entrega `CatalogoAutos` a los handlers. Ese parámetro no viene del usuario.
- `Results.Created` devuelve **201** y un encabezado **Location**.
- Una consulta de colección sin coincidencias devuelve **200 con []**; un auto inexistente devuelve **404**.
- `wwwroot/recursos/lectura.txt` se solicita como `/recursos/lectura.txt`.

## Archivos estáticos y versiones

`UseDefaultFiles()` reescribe `/` hacia un documento predeterminado, y `UseStaticFiles()` lo entrega. El orden importa. Se usa esta configuración para que la relación entre carpeta y URL sea sencilla de observar.

ASP.NET Core 9 y posteriores también ofrecen `MapStaticAssets()` para recursos descubiertos durante la compilación y su entrega optimizada. No se mezcla esa variante en este laboratorio; `UseStaticFiles()` sigue siendo válido en .NET 10. No coloque claves, contraseñas ni documentos privados en `wwwroot`.

## Alcance del caso Autos el campeón

El catálogo empieza con Toyota Corolla, Hyundai Accent y Honda Civic. Se conserva en memoria mediante un servicio singleton. Al reiniciar se pierden los nuevos registros. `ConcurrentDictionary` y `Interlocked` protegen operaciones individuales y la asignación local de identificadores; no sustituyen persistencia ni transacciones. Dos instancias tendrían catálogos separados.

Este proyecto es un laboratorio local. No incluye autenticación, autorización, ventas ni pagos. La validación se realiza explícitamente en los handlers para hacerla visible al estudiante. No se configura validación automática de Minimal APIs. Los precios se interpretan en CRC y se representan con `decimal`; el parámetro moneda del ejemplo aislado solo muestra binding, no realiza conversiones.

## Errores frecuentes

- **No encuentra net10.0:** instale el SDK 10 y confirme que la terminal lo reconoce.
- **Puerto ocupado:** use otro puerto con el comando anterior.
- **POST probado desde la barra del navegador:** la navegación normal envía GET; use la interfaz o el archivo HTTP.
- **404 en `/wwwroot/...`:** quite `wwwroot` de la URL.
- **400 antes del handler:** compruebe JSON y tipos de parámetros. En Development el cuerpo del error puede incluir detalles técnicos.
- **415:** el endpoint espera JSON, pero se envió otro Content-Type.
- **405:** la ruta existe para otro método; PUT `/api/autos` no está implementado.
- **Cambios de C# sin efecto:** reinicie `dotnet run`, o use `dotnet watch run` durante la clase.

## Documentación oficial

- https://learn.microsoft.com/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-10.0
- https://learn.microsoft.com/aspnet/core/fundamentals/minimal-apis/parameter-binding?view=aspnetcore-10.0
- https://learn.microsoft.com/aspnet/core/fundamentals/routing?view=aspnetcore-10.0
- https://learn.microsoft.com/aspnet/core/fundamentals/static-files?view=aspnetcore-10.0

## Verificación realizada

Compilado con SDK .NET 10: cero errores y cero advertencias. Se ejecutaron las 35 solicitudes de `Ejemplos.http`, verificando sus estados esperados. También se comprobó Location, consulta posterior de un auto creado, rechazo sin inserción de datos inválidos, colecciones vacías y acceso a archivos estáticos.
