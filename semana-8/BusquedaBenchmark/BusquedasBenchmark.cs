using BenchmarkDotNet.Attributes;

namespace BusquedaBenchmark;

public enum CasoBusqueda
{
    Inicio,
    Centro,
    Final,
    Ausente
}

[MemoryDiagnoser]
public class BusquedasBenchmark
{
    private int[] datos = [];
    private int objetivo;

    [Params(100, 10_000, 1_000_000)]
    public int Cantidad { get; set; }

    [ParamsAllValues]
    public CasoBusqueda Caso { get; set; }

    [GlobalSetup]
    public void PrepararDatos()
    {
        // Se genera una vez por caso, fuera de la medición.
        datos = Enumerable.Range(0, Cantidad).ToArray();
        objetivo = Caso switch
        {
            CasoBusqueda.Inicio => 0,
            CasoBusqueda.Centro => Cantidad / 2,
            CasoBusqueda.Final => Cantidad - 1,
            CasoBusqueda.Ausente => Cantidad,
            _ => throw new ArgumentOutOfRangeException(nameof(Caso))
        };
    }

    [Benchmark(Baseline = true)]
    public int Lineal() => AlgoritmosBusqueda.Lineal(datos, objetivo);

    [Benchmark]
    public int Binaria() => AlgoritmosBusqueda.Binaria(datos, objetivo);
}
