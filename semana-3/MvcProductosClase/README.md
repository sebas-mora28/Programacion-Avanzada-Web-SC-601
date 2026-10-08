# MVC Productos — ejemplo para clase

Requiere **SDK de .NET 10**. En la carpeta del proyecto ejecute:

```bash
dotnet run
```

Abra la URL indicada por la terminal (con el perfil incluido: `http://localhost:5080`). Los datos están en memoria: al reiniciar la aplicación vuelven los dos productos originales. No necesita paquetes NuGet adicionales ni base de datos.

## Recorrido sugerido

1. Abra `/Productos` y señale el método `Index` y `Views/Productos/Index.cshtml`.
2. Abra `/Productos/Detalle/1`: observe cómo el segmento `1` se enlaza al parámetro `id`.
3. Abra `/Productos/Crear`: GET muestra el formulario; POST recibe `Nombre` y `Precio`.
4. Pruebe un nombre vacío o un precio cero para mostrar validación y `ModelState.IsValid`.
5. Guarde un producto: el POST redirige a Index (patrón Post/Redirect/Get).
6. Edite y elimine un producto, observando las rutas y métodos HTTP.
7. Pida a los estudiantes que encuentren qué archivos modificarían para cambiar el HTML, la validación o el almacenamiento.

## Estructura

- `Models/Producto.cs`: datos y atributos de validación.
- `Controllers/ProductosController.cs`: acciones HTTP y respuestas (`View`, `RedirectToAction`, `NotFound`).
- `Views/Productos/*.cshtml`: vistas Razor tipadas con `@model`.
- `Services/ProductoRepository.cs`: almacenamiento en memoria; permite que el controlador no gestione directamente la lista.
- `Program.cs`: registro de MVC y ruta convencional.

El parámetro `[Bind("Nombre,Precio")]` evita enlazar `Id` desde el cuerpo del formulario; la acción de edición usa el `id` de la ruta. Es una demostración de limitar datos de entrada, no un sistema de autorización. El ejemplo no incluye autenticación ni persistencia.
