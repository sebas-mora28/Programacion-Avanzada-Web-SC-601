using BenchmarkDotNet.Running;
using BusquedaBenchmark;

if (args is ["--verificar"])
{
    Verificacion.Ejecutar();
    return;
}

BenchmarkSwitcher.FromAssembly(typeof(BusquedasBenchmark).Assembly).Run(args);
