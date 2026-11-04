#!/usr/bin/env node
'use strict';

const fs = require('node:fs');

const token = process.argv[2] ?? (process.stdin.isTTY ? '' : fs.readFileSync(0, 'utf8').trim());

function fail(message) {
  console.error(message);
  process.exitCode = 1;
}

function decodeJsonSegment(segment, name) {
  if (!segment || !/^[A-Za-z0-9_-]+$/.test(segment)) {
    throw new Error(`El segmento ${name} no tiene formato Base64URL válido.`);
  }

  try {
    return JSON.parse(Buffer.from(segment, 'base64url').toString('utf8'));
  } catch {
    throw new Error(`No se pudo decodificar el JSON del segmento ${name}.`);
  }
}

if (!token) {
  fail('Uso: node decode-jwt.js <token>  (o envíe el token por stdin).');
} else {
  const segments = token.split('.');

  if (segments.length !== 3) {
    fail('El token debe tener tres segmentos separados por puntos.');
  } else {
    try {
      const header = decodeJsonSegment(segments[0], 'header');
      const payload = decodeJsonSegment(segments[1], 'payload');

      console.log('Header:');
      console.log(JSON.stringify(header, null, 2));
      console.log('\nPayload:');
      console.log(JSON.stringify(payload, null, 2));
      console.log('\nLa firma no se valida; decodificar no comprueba que el token sea auténtico.');
    } catch (error) {
      fail(error.message);
    }
  }
}
