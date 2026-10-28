# PP2 — Mecanismos de Autenticación en ASP.NET Core
SC-701 · Sebastián Mora Godínez · Modalidad individual · 4–6 horas.

## Organización
Tres aplicaciones **independientes**, sin referencias entre proyectos, con los mismos dos usuarios ficticios. Cada app tiene un único esquema y proceso. No hay base de datos ni CRUD. CookiesApp usa MVC (controladores, modelos y vistas Razor). El servicio de usuarios y los DTO se duplican intencionalmente para permitir ejecutar cada app por separado.

| Proyecto | Mecanismo | URL HTTPS | Ruta principal |
|---|---|---|---|
| BasicApp | Basic | https://localhost:7101 | /api/basic/profile |
| CookiesApp | Cookies | https://localhost:7102 | /Login y /Dashboard |
| JwtApp | Bearer JWT | https://localhost:7103 | /api/auth/token y /api/jwt/profile |

## Requisitos y ejecución
Instalar SDK .NET 10 (no basta con el runtime) y una herramienta HTTP: Postman, REST Client de VS Code o equivalente. Internet se necesita para restaurar JwtBearer 10.0.0 y sus dependencias. `global.json` permite cualquier banda estable 10.0 posterior a 10.0.100.

Desde la carpeta que contiene `PP2.slnx`:
```sh
dotnet --version
dotnet dev-certs https --trust
dotnet restore PP2.slnx
dotnet build PP2.slnx
```
En Linux, la confianza del certificado depende del navegador/distribución; consultar la documentación oficial enlazada abajo. Usar HTTPS también en las pruebas. Los puertos HTTP 5101–5103 solo redirigen a HTTPS.

Ejecutar cada app en una terminal distinta (pueden iniciarse también de una en una):
```sh
dotnet run --project src/BasicApp
dotnet run --project src/CookiesApp
dotnet run --project src/JwtApp
```
También funciona `dotnet run` desde cada carpeta de proyecto. La raíz contiene tres proyectos: allí se debe especificar `--project`. Detener con Ctrl+C.

## Usuarios de prueba
| Usuario | Contraseña de laboratorio | Rol |
|---|---|---|
| admin | Admin123! | Admin |
| estudiante | User123! | User |

Los hashes precargados usan el formato Identity V3, PBKDF2-HMAC-SHA512, 210 000 iteraciones y sal aleatoria de 128 bits por usuario. `PasswordHasher` verifica las credenciales. Estas contraseñas se publican solo para las pruebas; no son credenciales reales. No modificar el servicio para aceptar cualquier contraseña ni devolver hashes. **Ambos usuarios pueden acceder a su perfil y al Dashboard**. En BasicApp, `/api/basic/admin` exige el rol `Admin`: el usuario `estudiante`, con rol `User`, debe recibir 403 al autenticarse correctamente.

## Estado inicial (no es la solución)
- Basic: arranca; el perfil y la ruta de administrador siempre responden 401. El challenge todavía no incluye WWW-Authenticate. Tras completar B1, las credenciales válidas permiten llegar al esqueleto del perfil, que devuelve 501 hasta completar B3. La ruta de administrador ya tiene la autorización por rol configurada: con B1 completo, responde 200 para `admin` y 403 para `estudiante`.
- Cookies: Login funciona como formulario; un envío válido muestra el aviso de TODO. Dashboard devuelve 501 como bloqueo provisional. Logout no cierra sesiones todavía.
- JWT: arranca sin clave; el perfil responde 401. Emisión con campos presentes devuelve 501, y con campos ausentes 400. `TokenService.Create` no se invoca en la base.
- Los 501 y avisos deben desaparecer al completar los TODO. No son resultados aceptables en la entrega final.

## Mapa de implementación
| TODO | Archivo (dentro de src) | Trabajo pendiente |
|---|---|---|
| B1 | BasicApp/Authentication/BasicAuthenticationHandler.cs | Leer, decodificar y validar Basic; construir ticket/claims |
| B2 | mismo archivo | Challenge 401 con WWW-Authenticate |
| B3 | BasicApp/Endpoints/BasicEndpoints.cs | Leer claims del usuario autenticado y devolver ProfileResponse con HTTP 200 |
| C1 | CookiesApp/Program.cs | Opciones de cookie y ruta de login |
| C2 | CookiesApp/Controllers/AccountController.cs (Login POST) | Validar e iniciar sesión; redirección local segura |
| C3 | CookiesApp/Controllers/DashboardController.cs | Autorización y sustitución del bloqueo provisional |
| C4 | CookiesApp/Controllers/AccountController.cs (LogoutPost) | Cerrar sesión mediante POST |
| J1 | JwtApp/Program.cs | Validar JWT: firma, emisor, audiencia y expiración |
| J2 | mismo archivo | Validar credenciales y emitir respuesta de token |
| J3 | JwtApp/Services/TokenService.cs | Construir y firmar JWT con expiración y claims |

Ayudas de API (no implementadas): `AuthenticationTicket`, `ClaimsIdentity`, `ClaimsPrincipal`, `SignInAsync`, `SignOutAsync`, `TokenValidationParameters`, `JwtSecurityToken`, `JwtSecurityTokenHandler`, `SigningCredentials` y `SymmetricSecurityKey`. Se pueden agregar los `using` necesarios. No crear endpoints ni vistas adicionales.

### Estructura MVC de CookiesApp

- `Controllers/AccountController.cs`: rutas `/`, `/Login` y `/Logout`; procesa los formularios y valida sus tokens antiforgery.
- `Controllers/DashboardController.cs`: ruta `/Dashboard`, con el bloqueo provisional de C3.
- `Models/LoginViewModel.cs`: datos y validación del formulario de inicio de sesión.
- `Views/Account/` y `Views/Dashboard/`: vistas Razor; `Views/Shared/` contiene el layout.
- `Services/UserService.cs`: servicio de validación de credenciales proporcionado.

`Program.cs` registra MVC con `AddControllersWithViews()` y publica las rutas por atributos con `MapControllers()`. La conversión conserva los TODO C1–C4 y las URL originales.

### Endpoints de BasicApp
Las rutas se registran en `Endpoints/BasicEndpoints.cs` mediante `app.MapBasicEndpoints()` en `Program.cs`.

| Método y ruta | Acceso | Trabajo del estudiante |
|---|---|---|
| `GET /` | Público | Ninguno; devuelve el estado de la base |
| `GET /api/basic/profile` | Autenticado con Basic | B3: devolver `id`, `username` y `role` desde los claims |
| `GET /api/basic/admin` | Autenticado con rol `Admin` | Comprobar 200 con `admin`, 403 con `estudiante` y 401 sin credenciales válidas |

Basic envía las credenciales en el encabezado `Authorization` de cada solicitud al perfil; esta aplicación no necesita un endpoint de login ni emite tokens. B1 valida las credenciales, B2 responde al intento de acceso sin autenticación válida y B3 construye la respuesta del perfil.

La ruta `/api/basic/admin` usa `RequireAuthenticatedUser()` y `RequireRole("Admin")`. B1 debe crear el claim `ClaimTypes.Role` con el rol del usuario validado. Si la autenticación falla, se ejecuta el challenge B2 (401); si la autenticación es válida pero falta el rol, el framework responde 403 mediante el manejo de acceso prohibido, sin ejecutar B2 ni el cuerpo del endpoint.

## Clave de firma JWT
La base deja `Jwt:SigningKey` vacía. Antes de probar su implementación, genere una clave local aleatoria. Configure la misma clave para emisión y validación. No incluya la clave real en el ZIP ni en las evidencias. Ejemplo de configuración mediante secretos de desarrollo:
```sh
dotnet user-secrets init --project src/JwtApp
dotnet user-secrets set "Jwt:SigningKey" "PEGUE_AQUI_UN_VALOR_ALEATORIO_DE_AL_MENOS_32_BYTES" --project src/JwtApp
```
El texto del ejemplo es un marcador, no una clave para usar. Puede generar 32 bytes aleatorios con `openssl rand -base64 32` (macOS/Linux). Alternativa PowerShell 7:
```powershell
[Convert]::ToBase64String([System.Security.Cryptography.RandomNumberGenerator]::GetBytes(32))
```
Use el texto generado como valor UTF-8 de la llave en ambos lugares. `user-secrets init` agrega un identificador al csproj; la clave se guarda fuera del repositorio. También es posible usar la variable de entorno `Jwt__SigningKey`. Al completar J1/J3, valide que la clave esté configurada y tenga longitud suficiente. HS256 es el algoritmo requerido para acotar esta práctica.

## Pruebas
1. Basic: ejecutar `src/BasicApp/requests.http`; comprobar códigos y WWW-Authenticate. En `/api/basic/admin`, verificar 200 con `admin`, 403 con `estudiante` y 401 sin credenciales o con contraseña incorrecta. Añadir credenciales sin separador y vacías. Un encabezado mal formado debe producir 401, nunca 500.
2. Cookies: abrir una ventana privada en https://localhost:7102/Dashboard. Debe redirigir a Login. Intentar contraseña incorrecta y luego una válida; comprobar nombre/rol. Pulsar Cerrar sesión; solicitar de nuevo /Dashboard y verificar redirección. No basta con volver atrás y mirar una captura almacenada por el navegador.
3. En las herramientas del navegador observar Set-Cookie, HttpOnly, Secure y SameSite. Conservar los formularios POST y sus tokens antiforgery. Para ver un 302 en clientes HTTP, desactivar seguimiento automático de redirecciones.
4. JWT: usar `src/JwtApp/requests.http`; copiar accessToken a `@token`. Comprobar ambos usuarios, credenciales inválidas, datos incompletos, ausencia de token y token mal formado. Alterar el payload conservando la firma y comprobar 401.
5. Expiración: poner temporalmente `ExpirationMinutes` en 1, reiniciar, emitir un token, usarlo inmediatamente (200), esperar más de 60 segundos y reutilizar exactamente ese token (401). Configurar `ClockSkew=TimeSpan.Zero`. Restaurar 5 minutos al finalizar.
6. Completar `EVIDENCIAS.md` con resultados reales y capturas breves; no escribir resultados esperados como si se hubieran observado.

## Documentación de la entrega
Nombre: PENDIENTE · Carné: PENDIENTE.
Añadir instrucciones finales, fuentes consultadas, uso de IA y respuestas a las preguntas del enunciado. Conservar la solución y excluir bin/obj, claves y tokens de la entrega. Los archivos HTTP contienen credenciales de laboratorio conocidas, no secretos de producción.

## Referencias oficiales
- Cookies: https://learn.microsoft.com/aspnet/core/security/authentication/cookie?view=aspnetcore-10.0
- JWT: https://learn.microsoft.com/aspnet/core/security/authentication/configure-jwt-bearer-authentication?view=aspnetcore-10.0
- Secretos: https://learn.microsoft.com/aspnet/core/security/app-secrets?view=aspnetcore-10.0
- HTTPS: https://learn.microsoft.com/aspnet/core/security/enforcing-ssl?view=aspnetcore-10.0

La emisión local de JWT se usa para comprender el mecanismo en este laboratorio; no se pide construir un proveedor de identidad de producción.
