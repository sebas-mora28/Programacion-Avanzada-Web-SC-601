# WebRootDemo — laboratorio ASP.NET Core

Proyecto didáctico en español para explicar Web Root, archivos estáticos y su convivencia con Minimal APIs. Requiere **SDK de .NET 10** (no solo el runtime). Sin paquetes NuGet externos, base de datos ni recursos de terceros.

## Ejecutar

Extrae el ZIP, abre una terminal en la carpeta que contiene WebRootDemo.csproj y ejecuta:

```bash
dotnet run
```

Abre http://localhost:5050. Detén con Ctrl+C. En VS Code puedes abrir esta carpeta; en Visual Studio abre WebRootDemo.csproj.

Si necesitas otro puerto:

```bash
dotnet run --no-launch-profile --urls http://localhost:5051
```

Ajusta también baseUrl en WebRootDemo.http. El HTTP local es para la demostración; una publicación real debe usar HTTPS.

## Archivos para explicar

| Archivo | Propósito |
|---|---|
| Program.cs | Configura los archivos estáticos y los endpoints de la API. |
| wwwroot/index.html | Página inicial estática. |
| wwwroot/css/site.css | Estilos descargados por el navegador. |
| wwwroot/js/site.js | JavaScript estático que usa fetch para consultar la API. |
| wwwroot/images/cine.svg | Imagen local, sin dependencia de Internet. |
| wwwroot/datos/ejemplo.json | JSON que se entrega directamente desde un archivo. |
| wwwroot/ayuda.html | Segunda página estática. |
| DatosInternos/nota.txt | Archivo fuera del web root que no está publicado. |
| WebRootDemo.http | Solicitudes de demostración; usar extensión compatible con archivos HTTP o Postman. |
| GUIA_CLASE.md | Secuencia de demostración, preguntas y ejercicio. |

## Ideas clave

- UseDefaultFiles reescribe / a /index.html; no entrega el archivo por sí mismo. Va antes de UseStaticFiles.
- UseStaticFiles entrega los recursos de wwwroot. No hace falta crear un MapGet por imagen.
- /css/site.css corresponde a wwwroot/css/site.css; no se escribe /wwwroot en la URL.
- MapGet crea endpoints dinámicos; sus rutas no necesitan un archivo físico equivalente.
- Un archivo JS se entrega como estático aunque produzca interacción dinámica en el navegador.
- La API tiene datos fijos en memoria, no persistencia.
- No hay controladores ni vistas Razor en este ejemplo.
- Los recursos se consideran públicos: no coloques secretos o documentos privados en wwwroot.
- En .NET 9+ también existe MapStaticAssets para recursos conocidos al compilar, con optimizaciones. Aquí se usa UseStaticFiles por sencillez y para observar archivos editados durante la demostración. No es necesario añadir ambos para esta práctica.

## Comprobaciones esperadas

/ entrega HTML; /css/site.css entrega CSS; /js/site.js entrega JavaScript; /images/cine.svg entrega la imagen. El botón de consulta devuelve tres películas o una con filtro Comedia. La hora cambia al consultar nuevamente. /api/peliculas/999 y /DatosInternos/nota.txt devuelven 404.

## Referencia

https://learn.microsoft.com/es-es/aspnet/core/fundamentals/static-files?view=aspnetcore-10.0

## Verificación del entregable

Se revisaron estructura, rutas de recursos, sintaxis JavaScript y contenido del ZIP. El entorno de preparación no dispone del SDK dotnet, por lo que no se realizó compilación ni ejecución del servidor. Ejecuta dotnet build y dotnet run en tu equipo antes de la clase.
