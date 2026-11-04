namespace BusquedaBenchmark;

public static class AlgoritmosBusqueda
{
    // Devuelve el índice del valor encontrado, o -1 si no existe.
    public static int Lineal(int[] datos, int objetivo)
    {
        for (int i = 0; i < datos.Length; i++)
        {
            if (datos[i] == objetivo)
                return i;
        }

        return -1;
    }

    // Requiere datos ordenados de menor a mayor.
    public static int Binaria(int[] datos, int objetivo)
    {
        int izquierda = 0;
        int derecha = datos.Length - 1;

        while (izquierda <= derecha)
        {
            int medio = izquierda + (derecha - izquierda) / 2;

            if (datos[medio] == objetivo)
                return medio;

            if (datos[medio] < objetivo)
                izquierda = medio + 1;
            else
                derecha = medio - 1;
        }

        return -1;
    }
}
