const express = require("express");

const app = express();
const PORT = 3000;

// Middleware para recibir JSON
app.use(express.json());

// Base de datos simulada en memoria
let productos = [
  {
    id: 1,
    nombre: "Laptop",
    precio: 850
  },
  {
    id: 2,
    nombre: "Mouse",
    precio: 25
  }
];

let siguienteId = 3;

// GET - Obtener todos los productos
app.get("/api/productos", (req, res) => {
  res.status(200).json(productos);
});

// GET - Obtener un producto por ID
app.get("/api/productos/:id", (req, res) => {
  const id = Number(req.params.id);

  const producto = productos.find((p) => p.id === id);

  if (!producto) {
    return res.status(404).json({
      mensaje: "Producto no encontrado"
    });
  }

  res.status(200).json(producto);
});

// POST - Crear un producto
app.post("/api/productos", (req, res) => {
  const { nombre, precio } = req.body;

  if (!nombre || precio === undefined) {
    return res.status(400).json({
      mensaje: "Los campos nombre y precio son obligatorios"
    });
  }

  const nuevoProducto = {
    id: siguienteId++,
    nombre,
    precio
  };

  productos.push(nuevoProducto);

  res.status(201).json(nuevoProducto);
});

// PUT - Actualizar completamente un producto
app.put("/api/productos/:id", (req, res) => {
  const id = Number(req.params.id);
  const { nombre, precio } = req.body;

  const indice = productos.findIndex((p) => p.id === id);

  if (indice === -1) {
    return res.status(404).json({
      mensaje: "Producto no encontrado"
    });
  }

  if (!nombre || precio === undefined) {
    return res.status(400).json({
      mensaje: "Los campos nombre y precio son obligatorios"
    });
  }

  productos[indice] = {
    id,
    nombre,
    precio
  };

  res.status(200).json(productos[indice]);
});

// DELETE - Eliminar un producto
app.delete("/api/productos/:id", (req, res) => {
  const id = Number(req.params.id);

  const indice = productos.findIndex((p) => p.id === id);

  if (indice === -1) {
    return res.status(404).json({
      mensaje: "Producto no encontrado"
    });
  }

  const productoEliminado = productos[indice];
  productos.splice(indice, 1);

  res.status(200).json({
    mensaje: "Producto eliminado correctamente",
    producto: productoEliminado
  });
});

// Ruta inicial
app.get("/", (req, res) => {
  res.json({
    mensaje: "API de productos funcionando"
  });
});

app.listen(PORT, () => {
  console.log(`Servidor ejecutándose en http://localhost:${PORT}`);
});
