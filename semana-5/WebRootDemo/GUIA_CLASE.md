# Guía del profesor — 20 a 30 minutos

## 1. Explorar el proyecto (5 min)

Abre Program.cs y wwwroot. Pregunta qué archivos espera descargar el navegador. Explica que Web Root es un directorio, no una Minimal API; puede usarse con distintos modelos de ASP.NET Core.

## 2. Observar solicitudes (5 min)

Ejecuta dotnet run. Abre http://localhost:5050 y las herramientas del navegador, pestaña Network. Desactiva temporalmente la caché con las herramientas abiertas y recarga. Identifica HTML, CSS, JavaScript y SVG; observa sus Content-Type. Cada recurso tiene su propia solicitud. No hay consulta inicial a la API hasta pulsar el botón.

## 3. Contrastar estático y dinámico (5 min)

Abre los enlaces de recursos; señala que sus URLs omiten wwwroot. Consulta el JSON estático. Pulsa el botón de hora dos veces: el servidor genera una respuesta nueva. Consulta la cartelera, aplica Comedia e inspecciona el JSON en Network. El JS se descargó como archivo, pero ahora ejecuta fetch y modifica el DOM.

## 4. Experimentos (5 min)

1. Modifica el color de fondo en site.css; guarda y recarga con caché desactivada.
2. Comenta UseStaticFiles, reinicia el servidor e intenta abrir /css/site.css y /api/peliculas. El CSS deja de servirse, pero la API sigue funcionando. La página tampoco se entrega.
3. Restaura UseStaticFiles. Comenta UseDefaultFiles y reinicia: / deja de abrir la página, mientras /index.html sí funciona.
4. Restaura ambas líneas. Abre /DatosInternos/nota.txt: responde 404 aunque el archivo exista en el proyecto.

## 5. Ejercicio (5 a 10 min)

Crea wwwroot/contacto.html, usa el CSS existente y agrega un enlace desde index.html. Agrega otra imagen local y muéstrala. Añade una película al arreglo de Program.cs, reinicia y comprueba el filtro. No es necesario crear un MapGet para contacto.html.

## Preguntas y respuestas

- ¿Por qué no se usa /wwwroot/images/cine.svg? Porque wwwroot es la raíz física de los archivos estáticos, no un segmento público de la URL.
- ¿Estático significa que JavaScript no puede cambiar la página? No: describe cómo se entrega el archivo.
- ¿El archivo fuera de wwwroot es accesible automáticamente? No; este proyecto no configura una ruta que lo publique.
- ¿Se pueden poner vistas cshtml en wwwroot para ejecutarlas? No; requieren el motor Razor y su configuración.
- ¿Qué diferencia hay entre /datos/ejemplo.json y /api/hora? El primero entrega un archivo; el segundo ejecuta código para generar la respuesta.
- ¿Guardar un archivo privado en wwwroot es apropiado? No: los recursos de esta práctica son públicos.
