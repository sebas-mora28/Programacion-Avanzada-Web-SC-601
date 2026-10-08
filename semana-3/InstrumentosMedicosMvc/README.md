# MediCatálogo: proyecto didáctico ASP.NET Core MVC

Requisito: SDK de .NET 10. Desde esta carpeta ejecutar `dotnet run`. Abrir la URL indicada en la consola.

## Recorrido para la clase

1. `Program.cs` registra MVC y define la ruta inicial `Productos/Index`.
2. `Models/Producto.cs` representa los datos y contiene un catálogo en memoria. Los cambios se pierden al reiniciar.
3. `Controllers/ProductosController.cs` obtiene productos del modelo y selecciona las vistas.
4. `Views/Productos/Index.cshtml` muestra `@if`, `@switch`, `@for` y una función con retorno definida en `@functions`.
5. `Views/Shared/_Layout.cshtml` rodea las vistas con navegación y pie; `@RenderBody()` inserta el contenido.
6. `Views/Shared/_Disponibilidad.cshtml` es una vista parcial reutilizada por Index y Detalle.
7. `Views/Shared/Error.cshtml` es una vista compartida para errores de servidor.

**Pregunta para estudiantes:** Si cambia la forma de mostrar la disponibilidad, ¿cuántos archivos hay que editar? ¿Dónde cambiarían los datos del catálogo?

Este proyecto usa condiciones sencillas para presentar información. Las decisiones de venta, autorización y actualización de inventario pertenecen al modelo o a la lógica de negocio, no a las vistas. No se incluye un ejemplo `void` que escriba HTML con `WriteLiteral`, porque para fragmentos reutilizables se muestra la vista parcial, que es más fácil de mantener.
