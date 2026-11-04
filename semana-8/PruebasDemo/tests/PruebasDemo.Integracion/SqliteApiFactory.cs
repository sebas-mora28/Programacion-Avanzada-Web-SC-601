using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using PruebasDemo.Api.Data;

namespace PruebasDemo.Integracion;

public class SqliteApiFactory : WebApplicationFactory<Program>
{
    // Cada fábrica tiene SU conexión: Data Source=:memory: no comparte datos entre fábricas.
    // SQLite es un motor relacional real, aunque aquí almacene sus datos en memoria.
    private readonly SqliteConnection connection = new("Data Source=:memory:");

    public SqliteApiFactory()
    {
        // La BD existe mientras esta conexión permanezca abierta. Si EF abriera/cerrara
        // una conexión nueva por consulta, se perderían el esquema y los datos de prueba.
        connection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureTestServices(services =>
        {
            // Quitamos las opciones Y el callback que aplica UseNpgsql.
            // Quitar solo DbContextOptions dejaría la configuración de PostgreSQL registrada.
            services.RemoveAll<AppDbContext>();
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<AppDbContext>>();
            services.AddDbContext<AppDbContext>(options => options.UseSqlite(connection));

            // Conservamos CotizacionService y ProductoRepository reales: solo cambia la BD.
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);
        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Construye el esquema SQLite y los HasData desde el modelo actual.
        // No ejecutamos las migraciones de PostgreSQL sobre un proveedor distinto.
        db.Database.EnsureCreated();
        return host;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing) connection.Dispose(); // Cerrar la conexión elimina la BD temporal.
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        await connection.DisposeAsync();
    }
}
