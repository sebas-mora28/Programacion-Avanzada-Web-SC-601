using Moq;
using PruebasDemo.Api.Models;
using PruebasDemo.Api.Repositories;
using PruebasDemo.Api.Services;

namespace PruebasDemo.Unitarias;

// Trait permite ejecutar solo este grupo con --filter "Categoria=Unitaria".
[Trait("Categoria", "Unitaria")]
public class CotizacionServiceTests
{
    // Theory ejecuta el mismo caso con cada fila de InlineData.
    // Probamos especialmente el límite: 4 unidades no tienen descuento; 5 sí.
    [Theory]
    [InlineData(1, 0, 100)]
    [InlineData(4, 0, 400)]
    [InlineData(5, 50, 450)]
    [InlineData(10, 100, 900)]
    [InlineData(100, 1000, 9000)]
    public async Task Cotizar_ConCantidadValida_CalculaDescuentoYTotal(
        int cantidad, int descuentoEsperado, int totalEsperado)
    {
        // ARRANGE: preparar datos y dependencias.
        // Strict detecta llamadas que no se configuraron explícitamente.
        var repository = new Mock<IProductoRepository>(MockBehavior.Strict);
        repository.Setup(r => r.ObtenerPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Producto(1, "Teclado", 100m));
        var service = new CotizacionService(repository.Object);

        // ACT: ejecutar la unidad REAL. No simulamos el servicio que queremos verificar.
        var resultado = await service.CotizarAsync(1, cantidad);

        // ASSERT: comprobar valores observables, no solo que el método no lanzó una excepción.
        Assert.NotNull(resultado);
        Assert.Equal(100m * cantidad, resultado.Subtotal);
        Assert.Equal((decimal)descuentoEsperado, resultado.Descuento);
        Assert.Equal((decimal)totalEsperado, resultado.Total);

        // Verify comprueba la interacción con la dependencia simulada.
        repository.Verify(r => r.ObtenerPorIdAsync(1, It.IsAny<CancellationToken>()), Times.Exactly(2));
        repository.VerifyNoOtherCalls();
    }

    // Fact representa un caso sin parámetros.
    [Fact]
    public async Task Cotizar_ConProductoInexistente_DevuelveNull()
    {
        // ARRANGE: Moq simula que no existe el producto, sin acceder a ningún almacenamiento.
        var repository = new Mock<IProductoRepository>(MockBehavior.Strict);
        repository.Setup(r => r.ObtenerPorIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Producto?)null);
        var service = new CotizacionService(repository.Object);

        // ACT
        var resultado = await service.CotizarAsync(999, 1);

        // ASSERT
        Assert.Null(resultado);
        repository.Verify(r => r.ObtenerPorIdAsync(999, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(101)]
    public async Task Cotizar_ConCantidadInvalida_LanzaExcepcionSinConsultarRepositorio(int cantidad)
    {
        // ARRANGE
        var repository = new Mock<IProductoRepository>(MockBehavior.Strict);
        var service = new CotizacionService(repository.Object);

        // ACT + ASSERT: ThrowsAsync espera la tarea y comprueba el tipo de excepción.
        var excepcion = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => service.CotizarAsync(1, cantidad));

        Assert.Equal("cantidad", excepcion.ParamName);
        repository.Verify(r => r.ObtenerPorIdAsync(
            It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Cotizar_ConDescuentoFraccionario_RedondeaACentimos()
    {
        // ARRANGE: subtotal 50.25; el 10 % es 5.025 y debe redondearse a 5.03.
        var repository = new Mock<IProductoRepository>(MockBehavior.Strict);
        repository.Setup(r => r.ObtenerPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Producto(1, "Producto de prueba", 10.05m));
        var service = new CotizacionService(repository.Object);

        // ACT
        var resultado = await service.CotizarAsync(1, 5);

        // ASSERT: un caso que diferencia AwayFromZero del redondeo al par.
        Assert.NotNull(resultado);
        Assert.Equal(50.25m, resultado.Subtotal);
        Assert.Equal(5.03m, resultado.Descuento);
        Assert.Equal(45.22m, resultado.Total);
    }
}
