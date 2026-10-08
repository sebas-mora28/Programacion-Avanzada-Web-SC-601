// Este archivo se sirve como estático, pero su código ejecuta acciones dinámicas.
const formulario = document.querySelector("#filtros");
const estado = document.querySelector("#estado");
const contenedor = document.querySelector("#peliculas");
formulario.addEventListener("submit", async (event) => {
  event.preventDefault();
  const boton = formulario.querySelector("button");
  const genero = document.querySelector("#genero").value;
  const url = genero ? `/api/peliculas?${new URLSearchParams({ genero })}` : "/api/peliculas";
  document.querySelector("#solicitud").textContent = url;
  estado.textContent = "Consultando…";
  boton.disabled = true;
  contenedor.replaceChildren();
  try {
    const respuesta = await fetch(url);
    if (!respuesta.ok) throw new Error(`HTTP ${respuesta.status}`);
    const peliculas = await respuesta.json();
    for (const pelicula of peliculas) {
      const tarjeta = document.createElement("article");
      tarjeta.className = "pelicula";
      const titulo = document.createElement("h3");
      titulo.textContent = pelicula.titulo;
      const detalle = document.createElement("p");
      detalle.textContent = `${pelicula.genero} · ${pelicula.duracion} min`;
      tarjeta.append(titulo, detalle);
      contenedor.append(tarjeta);
    }
    estado.textContent = peliculas.length ? `${peliculas.length} película(s) encontrada(s).` : "No hay películas para este filtro.";
  } catch (error) {
    estado.textContent = `No se pudo consultar la API: ${error.message}`;
  } finally {
    boton.disabled = false;
  }
});
document.querySelector("#ver-hora").addEventListener("click", async () => {
  const salida = document.querySelector("#hora");
  try {
    const respuesta = await fetch("/api/hora", { cache: "no-store" });
    if (!respuesta.ok) throw new Error(`HTTP ${respuesta.status}`);
    const datos = await respuesta.json();
    salida.textContent = datos.horaUtc;
  } catch (error) {
    salida.textContent = `No se pudo consultar la hora: ${error.message}`;
  }
});
