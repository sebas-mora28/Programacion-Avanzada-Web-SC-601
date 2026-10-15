using EFCore_Semana6.Data;
using EFCore_Semana6.Endpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Configure ConnectionStrings:DefaultConnection.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

app.MapGet("/", () => Results.Redirect("/api/tareas"));
app.MapTareasEndpoints();
app.MapCategoriasEndpoints();
app.MapUsuariosEndpoints();
app.MapReasignacionesEndpoints();

app.Run();
