# API CRUD con Express + Postman

Ejemplo sencillo para practicar métodos HTTP utilizando Express.

## 1. Instalar dependencias

```bash
npm install
```

## 2. Ejecutar el servidor

```bash
npm start
```

El servidor estará disponible en:

```text
http://localhost:3000
```

## Endpoints

### GET - Obtener todos los productos

```http
GET http://localhost:3000/api/productos
```

### GET - Obtener producto por ID

```http
GET http://localhost:3000/api/productos/1
```

### POST - Crear producto

```http
POST http://localhost:3000/api/productos
Content-Type: application/json
```

Body:

```json
{
  "nombre": "Teclado",
  "precio": 45
}
```

Respuesta esperada:

```json
{
  "id": 3,
  "nombre": "Teclado",
  "precio": 45
}
```

### PUT - Actualizar producto

```http
PUT http://localhost:3000/api/productos/1
Content-Type: application/json
```

Body:

```json
{
  "nombre": "Laptop Gaming",
  "precio": 1200
}
```

### DELETE - Eliminar producto

```http
DELETE http://localhost:3000/api/productos/2
```

## Códigos HTTP utilizados

- `200 OK`: solicitud procesada correctamente.
- `201 Created`: recurso creado correctamente.
- `400 Bad Request`: datos inválidos o incompletos.
- `404 Not Found`: recurso no encontrado.

## Nota

Los datos se almacenan únicamente en memoria. Si se reinicia el servidor, los productos regresan a sus valores iniciales.
