# Recorrido docente de Semana 5

## 1. Leer el arranque

Abra `Program.cs`. Explique `CreateBuilder`, `Build`, el registro de rutas y `Run`. Los handlers se registran al iniciar y se ejecutan cuando llega una solicitud compatible. Muestre la inscripción de `CatalogoAutos` como singleton y su recepción por parámetros en los endpoints.

**Pregunta:** ¿un parámetro del handler siempre viene de la solicitud?
**Respuesta:** no. Los servicios registrados pueden obtenerse del contenedor de dependencias.

## 2. MapGet

Pruebe saludo, curso, hora y sin-contenido. Compare `text/plain`, `application/json` y la ausencia de cuerpo con 204. Repita la consulta de hora.

**Pregunta:** ¿idempotencia significa que la respuesta nunca cambia?
**Respuesta:** no. Se refiere al efecto previsto de repetir la operación sobre el servidor. La hora puede cambiar sin que GET modifique el negocio.

## 3. MapPost

Envíe un mensaje y luego un mensaje vacío. Ejecute la cotización con descuento de 10 y de 120. Muestre que POST no implica necesariamente guardar un recurso. Enviar con POST no cifra datos: HTTPS protege el transporte.

**Pregunta:** ¿qué diferencia existe entre JSON válido y datos válidos?
**Respuesta:** `{"precio":-1}` es JSON válido, pero incumple la regla de precio positivo del catálogo.

## 4. Parámetros

Use las variantes de ruta, consulta, encabezado y ruta opcional. En búsqueda elimine pagina y luego pruebe `pagina=abc`. En encabezados cambie X-Sucursal. Relacione el cuerpo JSON de POST con el resto de orígenes.

**Pregunta:** ¿qué sucede con `/api/vehiculos?marca=Toyota#detalle`?
**Respuesta:** el fragmento `detalle` normalmente se procesa en el navegador y no se envía al servidor.

## 5. Restricciones frente a validación

Compare estas solicitudes sin cambiar otra cosa:

| Solicitud | Estado | Etapa |
|---|---|---|
| `/api/restricciones/entero/abc` | 404 | No coincide con int |
| `/api/restricciones/binding/abc` | 400 | Falla la conversión a int |
| `/api/restricciones/positivo/0` | 404 | No coincide con min(1) |
| `/api/restricciones/validacion/0` | 400 | El handler rechaza el valor |

Luego compare los identificadores entero y Guid y visite catalogo/destacados.

**Pregunta:** ¿gana siempre la primera ruta registrada?
**Respuesta:** no. Intervienen especificidad y restricciones. En el ejemplo, el literal destacados gana aunque fue registrado después de `{texto}`.

Una restricción de ruta no verifica autorización ni existencia en la base de datos. El ejemplo de longitud acepta seis caracteres, pero no prueba que ese código sea real.

## 6. Integración con Autos el campeón

1. Consulte `/api/autos` y observe los tres autos iniciales.
2. Filtre por Toyota y después por una marca inexistente; compare los arreglos.
3. Consulte el auto 1 y luego el 99999.
4. Cree un Kia Rio y observe 201 y Location.
5. Copie Location al campo de ruta, seleccione GET y consulte el nuevo auto.
6. Envíe precio negativo y marca vacía: observe errores por campo y compruebe que no se insertó.
7. Repita una creación válida: explique que se crean dos recursos diferentes.
8. Envíe PUT a la colección y observe 405.
9. Reinicie el proyecto para mostrar la diferencia entre memoria y persistencia.

## 7. Web root

Abra el logo y la lectura de texto. Muestre que se pueden descargar sin ejecutar los endpoints del catálogo. Quite temporalmente `UseStaticFiles`, reinicie y observe que los endpoints siguen funcionando pero la interfaz deja de publicarse. Restaure la llamada y reinicie.

**Pregunta:** ¿una página estática puede consultar una API?
**Respuesta:** sí. El archivo JavaScript se entrega como recurso estático y se ejecuta en el navegador, donde usa fetch.

## 8. Ejercicios de ampliación

- Añada un filtro opcional por modelo y agréguelo a una solicitud de `Ejemplos.http`.
- Implemente GET `/api/autos/resumen` con cantidad y precio promedio. Decida qué responder cuando la colección esté vacía.
- Cree una página `wwwroot/acerca.html` y explique su URL pública.
- Amplíe la validación para rechazar marcas compuestas solo por números y pruebe el comportamiento actual antes de modificarlo.
- Diseñe, sin programarla, una solución para conservar los autos al reiniciar.

Para cada cambio, pida un caso exitoso, un caso límite y uno inválido. No basta con mostrar una pantalla: el estudiante debe identificar método, ruta, origen de parámetros y estado esperado.
