const $ = (id) => document.getElementById(id);
let eventos = null;
let reloj = null;
let lineas = [];

function registrar(tipo, datos) {
  lineas.unshift(`[${new Date().toLocaleTimeString()}] ${tipo}\n${datos}`);
  lineas = lineas.slice(0, 50);
  $('registro').textContent = lineas.join('\n\n');
}

function conectar() {
  if (eventos) return; // Evitar conexiones duplicadas en la misma pestaña.
  $('conexion').textContent = 'Conectando…';
  $('conectar').disabled = true;
  $('desconectar').disabled = false;
  eventos = new EventSource('/eventos');

  eventos.onopen = () => {
    $('conexion').textContent = 'SSE conectado';
    registrar('Conexión', 'El servidor abrió el flujo.');
  };
  eventos.addEventListener('pedido', (event) => {
    const pedido = JSON.parse(event.data);
    $('estado').textContent = pedido.estado;
    $('detalle').textContent = `Versión ${pedido.version} · Actualizado: ${new Date(pedido.actualizadoEn).toLocaleTimeString()}`;
    registrar('pedido', event.data);
  });
  eventos.onerror = () => {
    if (eventos?.readyState === EventSource.CLOSED) {
      desconectar();
      $('conexion').textContent = 'Conexión cerrada. Puedes conectar otra vez.';
    } else {
      $('conexion').textContent = 'Sin conexión. Reintentando…';
      registrar('Conexión', 'EventSource intentará reconectar automáticamente.');
    }
  };
}

function desconectar() {
  eventos?.close(); // close() también detiene los intentos de reconexión.
  eventos = null;
  $('conexion').textContent = 'SSE desconectado';
  $('conectar').disabled = false;
  $('desconectar').disabled = true;
  registrar('Conexión', 'Flujo cerrado. La tarjeta conserva el último estado recibido.');
}

$('formulario').addEventListener('submit', async (event) => {
  event.preventDefault();
  $('enviar').disabled = true;
  try {
    const response = await fetch('/api/pedidos/42/estado', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ estado: $('nuevoEstado').value })
    });
    if (!response.ok) throw new Error(`HTTP ${response.status}: ${await response.text()}`);
    $('resultado').textContent = 'Cambio guardado. El servidor lo publicó por SSE.';
    // No actualizamos #estado con la respuesta del POST: lo hace el evento SSE.
  } catch (error) {
    $('resultado').textContent = `No se pudo enviar: ${error.message}`;
  } finally {
    $('enviar').disabled = false;
  }
});

$('conectar').addEventListener('click', conectar);
$('desconectar').addEventListener('click', desconectar);
$('limpiar').addEventListener('click', () => { lineas = []; $('registro').textContent = 'Sin eventos.'; });
$('relojBoton').addEventListener('click', () => {
  if (reloj) {
    reloj.close(); reloj = null;
    $('relojBoton').textContent = 'Iniciar reloj';
    $('reloj').textContent = 'Detenido';
    return;
  }
  reloj = new EventSource('/eventos/reloj');
  $('relojBoton').textContent = 'Detener reloj';
  reloj.addEventListener('reloj', (event) => {
    $('reloj').textContent = new Date(JSON.parse(event.data).hora).toLocaleTimeString();
    registrar('reloj', event.data);
  });
  reloj.onerror = () => { $('reloj').textContent = 'Sin conexión'; };
});
conectar();
