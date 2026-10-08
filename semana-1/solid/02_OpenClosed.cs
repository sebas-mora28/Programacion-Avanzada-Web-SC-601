namespace SolidExamples;

public static class OpenClosedExample
{
    public static void Run()
    {
        var calculadora = new CalculadoraPrecioFinal();
        

        Console.WriteLine();
        Console.WriteLine("2. OCP - Abierto/Cerrado");
        Console.WriteLine($"Cliente regular: {calculadora.Calcular(10000m, new DescuentoRegular()):C}");
        Console.WriteLine($"Cliente VIP: {calculadora.Calcular(10000m, new DescuentoVip()):C}");
    }
}

public interface IDescuento
{
    decimal Aplicar(decimal precio);
}

public sealed class DescuentoRegular : IDescuento
{
    public decimal Aplicar(decimal precio)
    {
        return precio * 0.95m;
    }
}

public sealed class DescuentoVip : IDescuento
{
    public decimal Aplicar(decimal precio)
    {
        return precio * 0.80m;
    }
}

public sealed class CalculadoraPrecioFinal
{
    public decimal Calcular(decimal precio, IDescuento descuento)
    {
        return descuento.Aplicar(precio);
    }
}
