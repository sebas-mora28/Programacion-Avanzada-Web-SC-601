namespace SolidExamples;

public static class SingleResponsibilityExample
{
    public static void Run()
    {
        var pedido = new Pedido("Ana", [12000m, 8500m, 4500m]);
        var calculadora = new CalculadoraTotalPedido();
        var impresora = new ImpresoraRecibo();

        var total = calculadora.CalcularTotal(pedido);

        Console.WriteLine();
        Console.WriteLine("1. SRP - Responsabilidad Unica");
        Console.WriteLine(impresora.CrearRecibo(pedido, total));
    }
}

public sealed class Pedido(string cliente, IReadOnlyList<decimal> precios)
{
    public string Cliente { get; } = cliente;
    public IReadOnlyList<decimal> Precios { get; } = precios;
}

public sealed class CalculadoraTotalPedido
{
    public decimal CalcularTotal(Pedido pedido)
    {
        return pedido.Precios.Sum();
    }
}

public sealed class ImpresoraRecibo
{
    public string CrearRecibo(Pedido pedido, decimal total)
    {
        return $"Cliente: {pedido.Cliente} | Total: {total:C}";
    }
}
