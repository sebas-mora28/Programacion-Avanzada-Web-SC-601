using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PruebasDemo.Api.Data;
using PruebasDemo.Api.Models;

namespace PruebasDemo.Integracion;

// xUnit crea una instancia de esta clase por prueba, incluyendo cada fila de InlineData.
// Cada instancia posee una fábrica y una base SQLite nuevas: no comparte cambios con otras pruebas.
// No hay Moq: endpoint, servicio y repositorio son reales; solo se sustituye PostgreSQL.
[Trait("Categoria", "Integracion")]
public class CotizacionesEndpointsTests : IDisposable
{
    private readonly SqliteApiFactory factory = new();

    public void Dispose() => factory.Dispose();

    [Theory]
    [InlineData(4, 0, 400)]
    [InlineData(5, 50, 450)]
    public async Task Post_ConDatosValidos_Devuelve200YLaCotizacionEnJson(
        int cantidad, int descuentoEsperado, int totalEsperado)
    {
        // ARRANGE: HttpClient envía solicitudes al servidor de pruebas.
        using var client = factory.CreateClient();

        // ACT: se recorren ruta, validación, handler, servicio, repositorio y consulta SQL.
        using var response = await client.PostAsJsonAsync("/api/cotizaciones",
            new { productoId = 1, cantidad });

        // ASSERT: comprobamos el contrato HTTP y el contenido calculado.
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        var cotizacion = await response.Content.ReadFromJsonAsync<Cotizacion>();
        Assert.NotNull(cotizacion);
        Assert.Equal(1, cotizacion.ProductoId);
        Assert.Equal("Teclado", cotizacion.Nombre);
        Assert.Equal(cantidad, cotizacion.Cantidad);
        Assert.Equal(100m, cotizacion.PrecioUnitario);
        Assert.Equal(100m * cantidad, cotizacion.Subtotal);
        Assert.Equal((decimal)descuentoEsperado, cotizacion.Descuento);
        Assert.Equal((decimal)totalEsperado, cotizacion.Total);
    }

    [Theory]
    [InlineData(1, 0, "Cantidad")]
    [InlineData(1, -1, "Cantidad")]
    [InlineData(1, 101, "Cantidad")]
    [InlineData(0, 1, "ProductoId")]
    public async Task Post_ConDatosInvalidos_Devuelve400ConErroresDeValidacion(
        int productoId, int cantidad, string campo)
    {
        // ARRANGE
        using var client = factory.CreateClient();

        // ACT: AddValidation procesa Range y rechaza la solicitud antes del handler.
        using var response = await client.PostAsJsonAsync("/api/cotizaciones",
            new { productoId, cantidad });

        // ASSERT: no basta comprobar 400; verificamos cuál propiedad causó el error.
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        var problema = await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
        Assert.NotNull(problema);
        Assert.True(problema.Errors.ContainsKey(campo));
    }

    [Fact]
    public async Task Post_ConProductoInexistente_Devuelve404()
    {
        // ARRANGE: 999 no pertenece al catálogo real de esta aplicación.
        using var client = factory.CreateClient();

        // ACT
        using var response = await client.PostAsJsonAsync("/api/cotizaciones",
            new { productoId = 999, cantidad = 1 });

        // ASSERT
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("El producto no existe.", body.GetProperty("mensaje").GetString());
    }

    [Fact]
    public async Task Post_ConJsonMalformado_Devuelve400()
    {
        // ARRANGE: este caso comprueba la deserialización de HTTP, no la regla de descuento.
        using var client = factory.CreateClient();
        using var content = new StringContent("{", Encoding.UTF8, "application/json");

        // ACT
        using var response = await client.PostAsync("/api/cotizaciones", content);

        // ASSERT
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_ConProductoGuardadoEnSqlite_ConsultaLosDatosPersistidos()
    {
        // ARRANGE: guardamos un producto nuevo, que no pertenece a los datos iniciales.
        using var client = factory.CreateClient();
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            Assert.True(db.Database.IsSqlite());
            db.Productos.Add(new Producto(42, "Producto SQLite", 10.05m));
            await db.SaveChangesAsync();
        } // Cerramos este DbContext: la solicitud usará otro, contra la misma conexión SQLite.

        // ACT: el repositorio debe encontrar el producto consultando realmente la base de datos.
        using var response = await client.PostAsJsonAsync("/api/cotizaciones",
            new { productoId = 42, cantidad = 5 });

        // ASSERT
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var cotizacion = await response.Content.ReadFromJsonAsync<Cotizacion>();
        Assert.NotNull(cotizacion);
        Assert.Equal("Producto SQLite", cotizacion.Nombre);
        Assert.Equal(10.05m, cotizacion.PrecioUnitario);
        Assert.Equal(5.03m, cotizacion.Descuento);
        Assert.Equal(45.22m, cotizacion.Total);
    }

    [Fact]
    public async Task FabricasDistintas_NoCompartenLosDatosDeSqlite()
    {
        // ARRANGE: creamos un producto en la base de la primera aplicación.
        using var client = factory.CreateClient();
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Productos.Add(new Producto(42, "Solo en la primera BD", 20m));
            await db.SaveChangesAsync();
        }
        using var otraFactory = new SqliteApiFactory();
        using var otroClient = otraFactory.CreateClient();

        // ACT: enviamos la misma solicitud a dos aplicaciones con bases independientes.
        using var primera = await client.PostAsJsonAsync("/api/cotizaciones",
            new { productoId = 42, cantidad = 1 });
        using var segunda = await otroClient.PostAsJsonAsync("/api/cotizaciones",
            new { productoId = 42, cantidad = 1 });

        // ASSERT: el registro existe solo en la primera base.
        Assert.Equal(HttpStatusCode.OK, primera.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, segunda.StatusCode);
    }

    [Fact]
    public async Task Post_SinPropiedades_Devuelve400EnAmbosCampos()
    {
        // ARRANGE
        using var client = factory.CreateClient();

        // ACT: las propiedades int omitidas quedan en cero y Range las rechaza.
        using var response = await client.PostAsJsonAsync("/api/cotizaciones", new { });

        // ASSERT
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problema = await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
        Assert.NotNull(problema);
        Assert.True(problema.Errors.ContainsKey("ProductoId"));
        Assert.True(problema.Errors.ContainsKey("Cantidad"));
    }
}
