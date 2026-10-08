# Idempotency-Key Demo

Este proyecto muestra un ejemplo sencillo de idempotencia usando el encabezado `Idempotency-Key`.

## ¿Qué hace?

Cuando un cliente envía la misma solicitud más de una vez con la misma clave, el servidor devuelve la misma respuesta sin volver a procesar el pago.

## Ejecutar

```bash
dotnet run
```

## Ejemplo de petición

```bash
curl -X POST http://localhost:5000/payments \
  -H "Content-Type: application/json" \
  -H "Idempotency-Key: pago-001" \
  -d '{"amount": 2500.00, "currency": "CRC"}'
```

## Resultado esperado

La primera vez se registra el pago y devuelve `201 Created`.
La segunda vez con la misma `Idempotency-Key` devuelve `200 OK` con `repeated = true` y el mismo resultado.
