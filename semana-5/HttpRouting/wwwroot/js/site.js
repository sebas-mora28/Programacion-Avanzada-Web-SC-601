const ejemplos = [
  {
    "group": "MapGet",
    "title": "Texto plano",
    "path": "/api/get/saludo",
    "description": "Un handler que devuelve un string.",
    "expected": "200 OK",
    "method": "GET",
    "body": null,
    "headers": {},
    "source": "Endpoints/GetEndpoints.cs"
  },
  {
    "group": "MapGet",
    "title": "Objeto JSON",
    "path": "/api/get/curso",
    "description": "Results.Ok devuelve un objeto serializado.",
    "expected": "200 OK",
    "method": "GET",
    "body": null,
    "headers": {},
    "source": "Endpoints/GetEndpoints.cs"
  },
  {
    "group": "MapGet",
    "title": "Idempotencia y hora",
    "path": "/api/get/hora",
    "description": "Repita el GET: la hora cambia aunque la operación no modifica datos del negocio.",
    "expected": "200 OK",
    "method": "GET",
    "body": null,
    "headers": {},
    "source": "Endpoints/GetEndpoints.cs"
  },
  {
    "group": "MapGet",
    "title": "Sin contenido",
    "path": "/api/get/sin-contenido",
    "description": "La respuesta no contiene cuerpo.",
    "expected": "204 No Content",
    "method": "GET",
    "body": null,
    "headers": {},
    "source": "Endpoints/GetEndpoints.cs"
  },
  {
    "group": "MapPost",
    "title": "Eco de un mensaje",
    "path": "/api/post/eco",
    "description": "Un DTO recibe el JSON del cuerpo.",
    "expected": "200 OK",
    "method": "POST",
    "body": {
      "mensaje": "Hola, clase"
    },
    "headers": {},
    "source": "Endpoints/PostEndpoints.cs"
  },
  {
    "group": "MapPost",
    "title": "Mensaje vacío",
    "path": "/api/post/eco",
    "description": "La validación del handler rechaza un mensaje vacío.",
    "expected": "400 Bad Request",
    "method": "POST",
    "body": {
      "mensaje": "  "
    },
    "headers": {},
    "source": "Endpoints/PostEndpoints.cs"
  },
  {
    "group": "MapPost",
    "title": "Calcular cotización",
    "path": "/api/post/cotizacion",
    "description": "POST procesa información sin crear un recurso.",
    "expected": "200 OK",
    "method": "POST",
    "body": {
      "precio": 10000000,
      "porcentajeDescuento": 10
    },
    "headers": {},
    "source": "Endpoints/PostEndpoints.cs"
  },
  {
    "group": "MapPost",
    "title": "Descuento fuera de rango",
    "path": "/api/post/cotizacion",
    "description": "La regla del negocio exige un porcentaje entre 0 y 100.",
    "expected": "400 Bad Request",
    "method": "POST",
    "body": {
      "precio": 10000000,
      "porcentajeDescuento": 120
    },
    "headers": {},
    "source": "Endpoints/PostEndpoints.cs"
  },
  {
    "group": "Parámetros",
    "title": "Valor en la ruta",
    "path": "/api/clientes/42",
    "description": "El segmento 42 se convierte a int.",
    "expected": "200 OK",
    "method": "GET",
    "body": null,
    "headers": {},
    "source": "Endpoints/ParametrosEndpoints.cs"
  },
  {
    "group": "Parámetros",
    "title": "Consulta y valores opcionales",
    "path": "/api/vehiculos?marca=Toyota&pagina=2",
    "description": "Cambie la consulta; quite los filtros para ver sus valores predeterminados.",
    "expected": "200 OK",
    "method": "GET",
    "body": null,
    "headers": {},
    "source": "Endpoints/ParametrosEndpoints.cs"
  },
  {
    "group": "Parámetros",
    "title": "Conversión fallida",
    "path": "/api/vehiculos?pagina=abc",
    "description": "El handler no se ejecuta si pagina no se convierte a entero.",
    "expected": "400 Bad Request",
    "method": "GET",
    "body": null,
    "headers": {},
    "source": "Endpoints/ParametrosEndpoints.cs"
  },
  {
    "group": "Parámetros",
    "title": "Encabezado personalizado",
    "path": "/api/inventario",
    "description": "FromHeader lee X-Sucursal. No se utiliza como prueba de identidad.",
    "expected": "200 OK",
    "method": "GET",
    "body": null,
    "headers": {
      "X-Sucursal": "Centro"
    },
    "source": "Endpoints/ParametrosEndpoints.cs"
  },
  {
    "group": "Parámetros",
    "title": "Ruta opcional",
    "path": "/api/sucursales",
    "description": "Compare con /api/sucursales/Heredia.",
    "expected": "200 OK",
    "method": "GET",
    "body": null,
    "headers": {},
    "source": "Endpoints/ParametrosEndpoints.cs"
  },
  {
    "group": "Parámetros",
    "title": "Ruta y consulta juntas",
    "path": "/api/vehiculos/8?moneda=USD",
    "description": "Este ejemplo muestra los valores recibidos; no calcula una conversión monetaria.",
    "expected": "200 OK",
    "method": "GET",
    "body": null,
    "headers": {},
    "source": "Endpoints/ParametrosEndpoints.cs"
  },
  {
    "group": "Restricciones",
    "title": "Entero válido",
    "path": "/api/restricciones/entero/12",
    "description": "La ruta acepta segmentos convertibles a int.",
    "expected": "200 OK",
    "method": "GET",
    "body": null,
    "headers": {},
    "source": "Endpoints/RestriccionesEndpoints.cs"
  },
  {
    "group": "Restricciones",
    "title": "Entero inválido",
    "path": "/api/restricciones/entero/abc",
    "description": "La restricción descarta esta ruta.",
    "expected": "404 Not Found",
    "method": "GET",
    "body": null,
    "headers": {},
    "source": "Endpoints/RestriccionesEndpoints.cs"
  },
  {
    "group": "Restricciones",
    "title": "Binding sin restricción",
    "path": "/api/restricciones/binding/abc",
    "description": "La ruta coincide, pero int id no puede recibir abc.",
    "expected": "400 Bad Request",
    "method": "GET",
    "body": null,
    "headers": {},
    "source": "Endpoints/RestriccionesEndpoints.cs"
  },
  {
    "group": "Restricciones",
    "title": "Mínimo como restricción",
    "path": "/api/restricciones/positivo/0",
    "description": "El valor 0 no cumple min(1): la ruta no coincide.",
    "expected": "404 Not Found",
    "method": "GET",
    "body": null,
    "headers": {},
    "source": "Endpoints/RestriccionesEndpoints.cs"
  },
  {
    "group": "Restricciones",
    "title": "Mínimo como validación",
    "path": "/api/restricciones/validacion/0",
    "description": "La ruta coincide y el handler explica el problema.",
    "expected": "400 Bad Request",
    "method": "GET",
    "body": null,
    "headers": {},
    "source": "Endpoints/RestriccionesEndpoints.cs"
  },
  {
    "group": "Restricciones",
    "title": "Identificador Guid",
    "path": "/api/restricciones/guid/550e8400-e29b-41d4-a716-446655440000",
    "description": "La restricción comprueba el formato, no la existencia.",
    "expected": "200 OK",
    "method": "GET",
    "body": null,
    "headers": {},
    "source": "Endpoints/RestriccionesEndpoints.cs"
  },
  {
    "group": "Restricciones",
    "title": "Longitud exacta",
    "path": "/api/restricciones/codigo/ABC123",
    "description": "Cambie el código a ABC y observe el 404.",
    "expected": "200 OK",
    "method": "GET",
    "body": null,
    "headers": {},
    "source": "Endpoints/RestriccionesEndpoints.cs"
  },
  {
    "group": "Restricciones",
    "title": "Desambiguar por entero",
    "path": "/api/restricciones/identificador/25",
    "description": "Compare con un Guid en el mismo lugar.",
    "expected": "200 OK",
    "method": "GET",
    "body": null,
    "headers": {},
    "source": "Endpoints/RestriccionesEndpoints.cs"
  },
  {
    "group": "Restricciones",
    "title": "Desambiguar por Guid",
    "path": "/api/restricciones/identificador/550e8400-e29b-41d4-a716-446655440000",
    "description": "Una plantilla similar selecciona otro handler por la restricción.",
    "expected": "200 OK",
    "method": "GET",
    "body": null,
    "headers": {},
    "source": "Endpoints/RestriccionesEndpoints.cs"
  },
  {
    "group": "Restricciones",
    "title": "Precedencia de un literal",
    "path": "/api/restricciones/catalogo/destacados",
    "description": "El literal gana frente a {texto}, aunque fue registrado después.",
    "expected": "200 OK",
    "method": "GET",
    "body": null,
    "headers": {},
    "source": "Endpoints/RestriccionesEndpoints.cs"
  },
  {
    "group": "Autos el campeón",
    "title": "Listar autos",
    "path": "/api/autos",
    "description": "El catálogo comienza con tres autos. Los nuevos se conservan hasta reiniciar.",
    "expected": "200 OK",
    "method": "GET",
    "body": null,
    "headers": {},
    "source": "Endpoints/AutosEndpoints.cs"
  },
  {
    "group": "Autos el campeón",
    "title": "Filtrar catálogo",
    "path": "/api/autos?marca=Toyota&precioMaximo=10000000",
    "description": "Combina dos filtros opcionales. Sin resultados devuelve [].",
    "expected": "200 OK",
    "method": "GET",
    "body": null,
    "headers": {},
    "source": "Endpoints/AutosEndpoints.cs"
  },
  {
    "group": "Autos el campeón",
    "title": "Consultar por id",
    "path": "/api/autos/1",
    "description": "Busca un vehículo existente.",
    "expected": "200 OK",
    "method": "GET",
    "body": null,
    "headers": {},
    "source": "Endpoints/AutosEndpoints.cs"
  },
  {
    "group": "Autos el campeón",
    "title": "Recurso inexistente",
    "path": "/api/autos/99999",
    "description": "La ruta coincide pero no hay un auto con ese id.",
    "expected": "404 Not Found",
    "method": "GET",
    "body": null,
    "headers": {},
    "source": "Endpoints/AutosEndpoints.cs"
  },
  {
    "group": "Autos el campeón",
    "title": "Crear auto",
    "path": "/api/autos",
    "description": "POST usa la misma ruta del listado. Location informa dónde consultar el nuevo auto.",
    "expected": "201 Created",
    "method": "POST",
    "body": {
      "marca": "Kia",
      "modelo": "Rio",
      "precio": 8000000
    },
    "headers": {},
    "source": "Endpoints/AutosEndpoints.cs"
  },
  {
    "group": "Autos el campeón",
    "title": "Validación de entrada",
    "path": "/api/autos",
    "description": "La respuesta contiene los errores por campo.",
    "expected": "400 Bad Request",
    "method": "POST",
    "body": {
      "marca": "",
      "modelo": "Rio",
      "precio": -1
    },
    "headers": {},
    "source": "Endpoints/AutosEndpoints.cs"
  },
  {
    "group": "Autos el campeón",
    "title": "Método no permitido",
    "path": "/api/autos",
    "description": "La colección tiene GET y POST, pero no PUT.",
    "expected": "405 Method Not Allowed",
    "method": "PUT",
    "body": {},
    "headers": {},
    "source": "Endpoints/AutosEndpoints.cs"
  },
  {
    "group": "Archivos estáticos",
    "title": "Archivo de texto",
    "path": "/recursos/lectura.txt",
    "description": "No se escribe wwwroot en la URL.",
    "expected": "200 OK",
    "method": "GET",
    "body": null,
    "headers": {},
    "source": "Program.cs y wwwroot/"
  },
  {
    "group": "Archivos estáticos",
    "title": "Prefijo incorrecto",
    "path": "/wwwroot/recursos/lectura.txt",
    "description": "La ruta física del archivo no es su URL pública.",
    "expected": "404 Not Found",
    "method": "GET",
    "body": null,
    "headers": {},
    "source": "Program.cs y wwwroot/"
  }
];
const $ = selector => document.querySelector(selector);
const grupos = [...new Set(ejemplos.map(e => e.group))];
for (const grupo of grupos) $('#grupo').add(new Option(grupo, grupo));
function cargarEjemplos() {
  $('#ejemplo').replaceChildren();
  ejemplos.forEach((ejemplo, indice) => {
    if (ejemplo.group === $('#grupo').value)
      $('#ejemplo').add(new Option(ejemplo.title, String(indice)));
  });
  elegirEjemplo();
}
function elegirEjemplo() {
  const e = ejemplos[Number($('#ejemplo').value)];
  $('#ruta').value = e.path;
  $('#metodo').value = e.method;
  $('#cuerpo').value = e.body === null ? '' : JSON.stringify(e.body, null, 2);
  $('#headers').value = JSON.stringify(e.headers, null, 2);
  $('#explicacion').textContent = e.description;
  $('#esperado').textContent = `Resultado esperado: ${e.expected}`;
  $('#archivo').textContent = e.source;
  actualizarCuerpo();
}
function actualizarCuerpo() { $('#cuerpo').disabled = $('#metodo').value === 'GET'; }
$('#grupo').addEventListener('change', cargarEjemplos);
$('#ejemplo').addEventListener('change', elegirEjemplo);
$('#metodo').addEventListener('change', actualizarCuerpo);
$('#enviar').addEventListener('click', async () => {
  const boton = $('#enviar');
  const inicio = performance.now();
  boton.disabled = true;
  $('#estado').textContent = 'Enviando…';
  $('#estado').className = '';
  $('#respuesta-headers').textContent = '—';
  $('#respuesta').textContent = '—';
  try {
    const url = new URL($('#ruta').value, location.origin);
    if (url.origin !== location.origin) throw new Error('Use una ruta de este laboratorio.');
    const encabezados = JSON.parse($('#headers').value || '{}');
    if (!encabezados || Array.isArray(encabezados) || typeof encabezados !== 'object')
      throw new Error('Los encabezados deben ser un objeto JSON.');
    const opciones = { method: $('#metodo').value, headers: new Headers(encabezados) };
    if (opciones.method !== 'GET') {
      opciones.headers.set('Content-Type', 'application/json');
      // Se permite JSON mal formado para observar el rechazo real del servidor.
      opciones.body = $('#cuerpo').value;
    }
    const respuesta = await fetch(url, opciones);
    const texto = await respuesta.text();
    $('#estado').textContent = `${respuesta.status} ${respuesta.statusText}`;
    $('#estado').className = respuesta.ok ? 'ok' : 'error';
    $('#detalle').textContent = `${opciones.method} ${url.pathname}${url.search} · ${Math.round(performance.now()-inicio)} ms`;
    $('#respuesta-headers').textContent = [...respuesta.headers]
      .map(([clave, valor]) => `${clave}: ${valor}`).join('\n') || '(Sin encabezados visibles)';
    let contenido = texto || '(Sin cuerpo de respuesta)';
    try { contenido = JSON.stringify(JSON.parse(texto), null, 2); } catch { /* Puede ser texto. */ }
    $('#respuesta').textContent = contenido;
  } catch (error) {
    $('#estado').textContent = 'Error del cliente o de red';
    $('#estado').className = 'error';
    $('#detalle').textContent = 'La solicitud no se pudo completar.';
    $('#respuesta').textContent = error.message;
  } finally { boton.disabled = false; }
});
cargarEjemplos();
