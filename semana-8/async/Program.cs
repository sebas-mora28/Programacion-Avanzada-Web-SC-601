using System;
using System.Threading.Tasks;
using System.Diagnostics;

class Program
{
    // Main devuelve Task para que el programa espere las operaciones asíncronas antes de salir.
    static async Task Main()
    {
        Console.WriteLine("Iniciando tareas... usando when.All");

        // Ambas preparaciones empiezan antes de esperarlas.
        // Cada llamada ejecuta su código hasta llegar a un await que todavía no ha terminado.
        Task tarea1 = PrepararCafe();
        Task tarea2 = PrepararTostadas();

        var stopWatch = Stopwatch.StartNew();
        // WhenAll espera las dos tareas; no crea un hilo dedicado para cada una.
        // await permite esperar su finalización sin bloquear un hilo durante la espera.
        await Task.WhenAll(tarea1, tarea2);
        stopWatch.Stop();


        Console.WriteLine($"Tiempo total: {stopWatch.Elapsed.TotalSeconds:N2} segundos.");
        Console.WriteLine("Desayuno listo!\n\n");


        // Console.WriteLine("Iniciando tareas... secuencialmente");
        // var stopWatch2 = Stopwatch.StartNew();
        // await PrepararCafe();
        // await PrepararTostadas();
        // stopWatch2.Stop();
        // Console.WriteLine($"Tiempo total secuencial: {stopWatch2.Elapsed.TotalSeconds:N2} segundos.");
    }

    static async Task PrepararCafe()
    {
        Console.WriteLine("Preparando cafe...");

        // Simula 3 segundos de preparación sin bloquear un hilo como haría Thread.Sleep.
        await Task.Delay(3000);

        Console.WriteLine("Cafe listo!");
    }

    static async Task PrepararTostadas()
    {
        Console.WriteLine("Preparando tostadas...");

        // Esta espera ocurre mientras también se prepara el café.
        await Task.Delay(3000);

        Console.WriteLine("Tostadas listas!");
    }
}
