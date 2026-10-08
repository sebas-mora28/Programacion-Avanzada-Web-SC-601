# Server-Sent Events con ASP.NET Core 10

Proyecto didáctico de Minimal APIs, con una página HTML y JavaScript sin dependencias externas. Demuestra cómo el servidor envía eventos al navegador sin recargar la página.

## Requisitos y ejecución

Instala el **SDK de .NET 10** (el runtime por sí solo no basta). Compruébalo con `dotnet --list-sdks`.

Desde la carpeta que contiene `SseDemo.csproj`:

```bash
dotnet run
```

Abre **http://localhost:5050**. Si tu editor no utiliza el perfil de ejecución:

```bash
dotnet run --no-launch-profile --urls http://localhost:5050
```

No requiere base de datos, npm ni paquetes NuGet adicionales. En Visual Studio abre `SseDemo.csproj`. En VS Code abre esta carpeta y ejecuta el comando en la terminal.

## Demostración en clase (15–20 minutos)

1. Abre la página en dos pestañas. Ambas reciben el estado inicial “Recibido”.
2. Cambia el estado a “En preparación” en la primera pestaña. La segunda también se actualiza sin recargarse.
3. En DevTools > Network, encuentra `/eventos`. La petición permanece abierta. Inspecciona su respuesta o el panel EventStream si está disponible.
4. Envía otro cambio. Identifica `event: pedido`, `data:` y `retry:`. Cada mensaje termina con una línea vacía.
5. Pulsa **Desconectar SSE** en la segunda pestaña. Cambia el pedido en la primera: la segunda conserva el estado anterior.
6. Pulsa **Conectar SSE**. Recibe inmediatamente el estado actual. No reproduce todas las transiciones intermedias.
7. Inicia el reloj. `/eventos/reloj` abre un flujo independiente que produce un valor cada tres segundos. Detén el reloj para cerrar esa conexión.
8. Con una pestaña conectada, detén el servidor con Ctrl+C y ejecútalo otra vez. EventSource intenta reconectar. Como los datos están en memoria, el pedido vuelve a “Recibido”.

**Diferencia clave:** un corte involuntario puede iniciar reconexión automática. `EventSource.close()` cierra voluntariamente la instancia y detiene sus reintentos.

## Orden sugerido para explicar el código

1. `Program.cs`, endpoint `/eventos/reloj`: el ejemplo mínimo usa `IAsyncEnumerable`, `yield return`, `Task.Delay` y `TypedResults.ServerSentEvents`.
2. `wwwroot/app.js`: `new EventSource(...)`, `addEventListener`, `onopen`, `onerror` y `close`.
3. `Program.cs`, endpoint POST: el navegador envía acciones mediante solicitudes HTTP normales.
4. `Services/PedidoService.cs`: cada cliente tiene su propio canal y recibe una copia de cada actualización.
5. El bloque `finally`: retira la suscripción cuando finaliza la conexión.

`CancellationToken` permite cancelar la espera cuando el cliente abandona la petición. El temporizador del reloj simula un productor: el flujo de pedidos depende de cambios reales enviados mediante POST.

La tarjeta del pedido **solo se actualiza desde el listener SSE**. El POST no modifica directamente la tarjeta. Esto permite mostrar la diferencia entre enviar una acción y recibir su notificación.

## Estructura

- `SseDemo.csproj`: proyecto web dirigido a .NET 10.
- `Program.cs`: configuración, archivos estáticos y endpoints Minimal API.
- `Models/Pedido.cs`: estado del pedido y cuerpo del POST.
- `Services/PedidoService.cs`: almacenamiento en memoria y distribución a suscriptores.
- `wwwroot/index.html`: interfaz de la demostración.
- `wwwroot/app.js`: cliente SSE y envío de acciones.
- `wwwroot/styles.css`: presentación visual.
- `SseDemo.http`: solicitudes para ejecutar desde un cliente compatible.
- `Properties/launchSettings.json`: perfil local en el puerto 5050.

## Endpoints

| Método | Ruta | Propósito |
| --- | --- | --- |
| GET | `/` | Interfaz de la demostración |
| GET | `/api/pedidos/42` | Consultar el estado actual como JSON |
| POST | `/api/pedidos/42/estado` | Cambiar y publicar el estado |
| GET | `/eventos` | Flujo de eventos `pedido` |
| GET | `/eventos/reloj` | Flujo de eventos `reloj` cada 3 segundos |

Estados admitidos: `Recibido`, `En preparación`, `En camino`, `Entregado`. Para facilitar la demostración, pueden seleccionarse en cualquier orden. Cada POST válido incrementa la versión, incluso si repite el estado.

## Prueba desde terminal

Mantén este comando abierto para ver los eventos conforme llegan:

```bash
curl -N http://localhost:5050/eventos
```

Desde otra terminal:

```bash
curl -X POST http://localhost:5050/api/pedidos/42/estado \
  -H 'Content-Type: application/json' \
  -d '{"estado":"En camino"}'
```

En PowerShell usa `curl.exe` o el archivo `SseDemo.http`. La opción `-N` desactiva el buffering de salida de curl.

## Verificación manual

| Acción | Resultado esperado |
| --- | --- |
| Abrir dos pestañas | Ambas muestran “Recibido” y versión 1 al inicio |
| Cambiar un estado | Ambas reciben la misma versión nueva |
| Enviar un estado desconocido | HTTP 400, sin publicar un cambio |
| Desconectar una pestaña | Deja de recibir actualizaciones |
| Reconectarla | Recibe el estado actual |
| Reiniciar el servidor | Reconecta y muestra datos iniciales, pues son volátiles |
| Iniciar y detener reloj | Los mensajes periódicos comienzan y se detienen |

## Decisiones y alcance

- **Un canal por suscriptor:** si varios clientes consumieran un único `Channel`, competirían por los mensajes en vez de recibir todos una copia.
- **Estado completo:** cada evento lleva el pedido completo. Al reconectar se envía una instantánea actual. Esta demo no usa `Last-Event-ID` ni ofrece replay o entrega exactamente una vez.
- **Memoria limitada:** cada canal almacena hasta 32 estados. Para un cliente lento descarta los más antiguos. Es apropiado para esta demostración de estado actual, pero no para auditoría ni para procesos que necesitan cada transición.
- **Suscripción atómica:** registrar el canal y tomar el estado inicial ocurre bajo el mismo lock. También se serializan las actualizaciones para preservar el orden dentro del proceso.
- **Una sola instancia:** reiniciar pierde los datos. Varias réplicas necesitarían persistencia y un mecanismo de distribución de eventos.
- **Uso local:** no incluye autenticación. Todas las pestañas comparten el pedido 42. En producción se necesita HTTPS, autorización de cada suscripción y protección de los endpoints de escritura.
- **Infraestructura:** el flujo de pedidos permanece silencioso si no hay cambios. No incluye heartbeats. Si se publica detrás de un proxy, deben revisarse buffering, timeouts y el envío periódico de comentarios o eventos de mantenimiento. El reloj es un flujo separado y no mantiene vivo el flujo del pedido.
- **Mismo origen:** la página se sirve desde ASP.NET Core. No la abras como un archivo `file://`.

## Preguntas de discusión

- ¿Qué parte envía la acción del usuario y cuál recibe el resultado?
- ¿Por qué la solicitud SSE aparece pendiente en DevTools?
- ¿Cuál sería el efecto de usar un solo canal compartido?
- ¿Qué cambia si necesitamos guardar y reproducir cada transición?
- ¿Cuándo elegiríamos polling o WebSocket para este mismo problema?

## Referencias

- https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/responses?view=aspnetcore-10.0#server-sent-events-sse
- https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.typedresults.serversentevents?view=aspnetcore-10.0
- https://developer.mozilla.org/en-US/docs/Web/API/Server-sent_events/Using_server-sent_events
- https://html.spec.whatwg.org/multipage/server-sent-events.html

## Validación del archivo entregado

Se revisaron la estructura del proyecto, la sintaxis de JavaScript y la configuración JSON/XML. No se ejecutó `dotnet build` ni la aplicación porque el entorno de creación no dispone del SDK de .NET. Ejecuta `dotnet build` y la verificación manual anterior en tu equipo.
