namespace BusquedaBenchmark;

internal static class Verificacion
{
    public static void Ejecutar()
    {
        int comprobaciones = 0;
        int[][] arreglos = [[], [4], [-10, -3, 0, 4, 9], [2, 2, 2, 5, 8, 8]];

        foreach (int[] datos in arreglos)
        {
            for (int objetivo = -11; objetivo <= 10; objetivo++)
            {
                bool existe = Array.IndexOf(datos, objetivo) >= 0;
                foreach (var buscar in new Func<int[], int, int>[]
                {
                    AlgoritmosBusqueda.Lineal,
                    AlgoritmosBusqueda.Binaria
                })
                {
                    int indice = buscar(datos, objetivo);
                    bool correcto = existe
                        ? indice >= 0 && indice < datos.Length && datos[indice] == objetivo
                        : indice == -1;

                    if (!correcto)
                        throw new InvalidOperationException($"Falló {buscar.Method.Name} para {objetivo}.");

                    comprobaciones++;
                }
            }
        }

        foreach (int cantidad in new[] { 100, 10_000, 1_000_000 })
        {
            foreach (CasoBusqueda caso in Enum.GetValues<CasoBusqueda>())
            {
                var benchmark = new BusquedasBenchmark { Cantidad = cantidad, Caso = caso };
                benchmark.PrepararDatos();
                int esperado = caso switch
                {
                    CasoBusqueda.Inicio => 0,
                    CasoBusqueda.Centro => cantidad / 2,
                    CasoBusqueda.Final => cantidad - 1,
                    _ => -1
                };

                if (benchmark.Lineal() != esperado || benchmark.Binaria() != esperado)
                    throw new InvalidOperationException($"Falló el escenario {cantidad}/{caso}.");

                comprobaciones += 2;
            }
        }

        Console.WriteLine($"Correcto: {comprobaciones} comprobaciones de búsqueda superadas.");
    }
}
