# Desayuno asíncrono en consola

Programa de consola con .NET 10 basado en el ejemplo de preparar café y tostadas. Toda la lógica está en `Program.cs`, con comentarios sobre `Task`, `async`, `await` y `Task.WhenAll`.

## Ejecutar

Desde la raíz del repositorio:

```bash
dotnet run --project semana-8/DesayunoAsincrono
```

La salida habitual es:

```text
Iniciando tareas...
Preparando cafe...
Preparando tostadas...
Tostadas listas!
Cafe listo!
Desayuno listo!
```

El café tarda aproximadamente 3 segundos y las tostadas 2. Como ambas tareas se inician antes de `await Task.WhenAll`, sus esperas se superponen y el programa termina en unos 3 segundos, más el tiempo de inicio y ejecución.

`Task.Delay` simula una espera sin bloquear un hilo. `Task.WhenAll` espera a que terminen ambas operaciones; no crea un hilo por tarea. El mensaje final aparece después de que las dos preparaciones estén listas.

Para comparar con una ejecución secuencial, sustituya las dos variables y `Task.WhenAll` por:

```csharp
await PrepararCafe();
await PrepararTostadas();
```

En ese caso, las tostadas empiezan después del café y el total se aproxima a 5 segundos. Ambas versiones usan métodos asíncronos; cambia el momento en que se inicia cada tarea.
