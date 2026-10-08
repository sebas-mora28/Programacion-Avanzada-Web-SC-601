namespace SolidExamples;

public static class InterfaceSegregationExample
{
    public static void Run()
    {
        IImpresora impresora = new ImpresoraBasica();
        IImpresoraEscaner multifuncional = new ImpresoraMultifuncional();

        Console.WriteLine();
        Console.WriteLine("4. ISP - Segregacion de Interfaces");
        Console.WriteLine(impresora.Imprimir("Factura"));
        Console.WriteLine(multifuncional.Escanear("Contrato"));
    }
}

public interface IImpresora
{
    string Imprimir(string documento);
}

public interface IEscaner
{
    string Escanear(string documento);
}

public interface IImpresoraEscaner : IImpresora, IEscaner
{
}

public sealed class ImpresoraBasica : IImpresora
{
    public string Imprimir(string documento)
    {
        return $"Imprimiendo: {documento}";
    }
}

public sealed class ImpresoraMultifuncional : IImpresoraEscaner
{
    public string Imprimir(string documento)
    {
        return $"Imprimiendo: {documento}";
    }

    public string Escanear(string documento)
    {
        return $"Escaneando: {documento}";
    }
}
