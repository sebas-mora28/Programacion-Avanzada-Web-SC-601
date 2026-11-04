# Comparación de búsquedas con BenchmarkDotNet

Aplicación de consola en .NET 10 para comparar búsqueda lineal y binaria con BenchmarkDotNet 0.15.8.

## Ejecutar

Desde la raíz del repositorio, con el SDK de .NET 10 instalado:

```bash
cd semana-8/BusquedaBenchmark
dotnet run -c Release -- --filter '*'
```

La primera ejecución restaura los paquetes de NuGet. La medición completa puede tardar varios minutos. Use `Release` y ejecute sin depurador, como indica la [guía oficial de BenchmarkDotNet](https://benchmarkdotnet.org/articles/guides/getting-started.html).

Para una demostración más corta, con menos iteraciones:

```bash
dotnet run -c Release -- --filter '*' --job short
```

Para comprobar únicamente que todos los escenarios se ejecutan:

```bash
dotnet run -c Release -- --filter '*' --job Dry
```

`Dry` sirve como prueba de ejecución; sus tiempos incluyen efectos de arranque y no permiten sacar conclusiones de rendimiento.

Para verificar resultados sin medir tiempos:

```bash
dotnet run -c Release -- --verificar
```

Esta verificación cubre arreglos vacíos, un solo elemento, valores negativos, duplicados, valores ausentes y los escenarios usados por el benchmark. Ambos algoritmos devuelven un índice válido o `-1` si el valor no existe; con duplicados, la búsqueda binaria puede devolver cualquiera de las coincidencias.

## Qué se compara

| Algoritmo | Funcionamiento | Complejidad temporal en el peor caso |
|---|---|---|
| Lineal | Recorre los elementos hasta encontrar el valor. | O(n) |
| Binaria | Descarta la mitad del intervalo en cada paso. | O(log n) |

Se utilizan arreglos ordenados de **100, 10 000 y 1 000 000** de enteros, con valores de `0` a `Cantidad - 1`. Cada tamaño se combina con cuatro casos: inicio, centro, final y ausente (un valor mayor que todos los elementos). Son **24 benchmarks**: dos algoritmos × tres tamaños × cuatro casos.

La búsqueda binaria requiere datos ordenados. `[GlobalSetup]` crea el arreglo y selecciona el objetivo fuera de la medición: se compara únicamente el costo de buscar en datos ya preparados. Si sus datos necesitan ordenarse primero, deberá considerar también ese costo al elegir un algoritmo.

Cada método marcado con `[Benchmark]` devuelve el resultado para que BenchmarkDotNet lo consuma. La búsqueda lineal es la referencia (`Baseline = true`) y `[MemoryDiagnoser]` registra las asignaciones de memoria por operación.

## Leer los resultados

| Columna | Significado |
|---|---|
| `Mean` | Tiempo promedio por búsqueda; menor es mejor. |
| `Error` / `StdDev` | Incertidumbre y variación de las mediciones. |
| `Ratio` | Tiempo relativo a la búsqueda lineal del mismo tamaño y caso. `0.10` equivale aproximadamente a una décima parte del tiempo. |
| `Allocated` | Memoria administrada asignada por búsqueda; no incluye el arreglo creado en la preparación. |

Los reportes quedan en `BenchmarkDotNet.Artifacts/results/` dentro del directorio desde el que ejecutó el comando. Los tiempos dependen del equipo y de su carga; no se incluyen resultados prefijados.

La búsqueda lineal puede ser más rápida cuando el objetivo está al inicio. La ventaja de la binaria suele crecer en arreglos grandes cuando la lineal debe recorrer muchos elementos. Estos casos repiten el mismo objetivo sobre el mismo arreglo: ilustran escenarios concretos y no representan una carga de búsquedas aleatorias.

## Archivos para revisar en clase

- `AlgoritmosBusqueda.cs`: implementación de ambos algoritmos.
- `BusquedasBenchmark.cs`: parámetros, preparación y métodos medidos.
- `Program.cs`: inicio de BenchmarkDotNet y recepción de argumentos.
- `Verificacion.cs`: comprobaciones de exactitud independientes del tiempo.
