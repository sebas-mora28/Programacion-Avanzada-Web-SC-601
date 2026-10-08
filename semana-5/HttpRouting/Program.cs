using Semana05HttpRouting.Endpoints;
using Semana05HttpRouting.Services;

var builder = WebApplication.CreateBuilder(args);
// Una sola instancia del catálogo para todas las solicitudes de este proceso.
builder.Services.AddSingleton<CatalogoAutos>();
var app = builder.Build();

// / se reescribe a /index.html; luego se entrega desde wwwroot.
app.UseDefaultFiles();
app.UseStaticFiles();

// Métodos propios de extensión: cada archivo agrupa un tema de la clase.
app.MapEjemplosGet();
app.MapEjemplosPost();
app.MapEjemplosParametros();
app.MapEjemplosRestricciones();
app.MapAutos();

// HTTP local intencional para simplificar la clase. En producción configure HTTPS.
app.Run();
