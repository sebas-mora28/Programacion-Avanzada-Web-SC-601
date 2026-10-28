# Práctica – Revisión del diseño HTTP de una Minimal API

## Contexto

Una empresa dispone de una API para administrar un catálogo de productos. La aplicación ya funciona, pero se debe revisar el contrato HTTP y corregir los problemas encontrados. **No se requiere base de datos ni vistas**.

## Requisitos

- SDK de .NET 10
- Visual Studio Code o editor equivalente
- Cliente HTTP (archivo `requests.http`, curl o Postman)

## Ejecución

Desde la carpeta del proyecto:

```bash
dotnet restore
dotnet run --launch-profile http
```

URL base: `http://localhost:5081`.

## Actividades

1. Revise los cinco endpoints existentes y determine qué operaciones consultan o modifican recursos.
2. Corrija los métodos HTTP según la semántica de cada operación. La API final debe utilizar `GET /products`, `GET /products/{id}`, `POST /products`, `PUT /products/{id}` y `DELETE /products/{id}`.
3. Aplique respuestas HTTP correctas: `200 OK` para consultas exitosas, `201 Created` al crear recursos, `204 No Content` al eliminar correctamente, `400 Bad Request` para solicitudes inválidas y `404 Not Found` para productos inexistentes. Para actualizaciones exitosas puede utilizar `200 OK` o `204 No Content`.
4. Al crear un producto, incluya el encabezado `Location` apuntando a `/products/{id}`.
5. Rechace nombres vacíos o compuestos únicamente por espacios, y precios negativos.
6. Asegúrese de que repetir el mismo `PUT` con el mismo cuerpo no incremente progresivamente el precio del producto.
7. Actualice `requests.http` con solicitudes para los nuevos endpoints y evidencie casos exitosos y de error.

## Entregable

- Proyecto .NET corregido.
- Archivo `requests.http` actualizado.
- Breve explicación de **tres** problemas encontrados y cómo se corrigieron.

## Notas

Los productos se almacenan en memoria y se reinician al detener la aplicación. No se requiere implementar autenticación, base de datos, páginas ni un CRUD nuevo desde cero: debe **corregirse la API existente**.
