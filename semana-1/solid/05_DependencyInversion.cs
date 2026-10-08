namespace SolidExamples;

public static class DependencyInversionExample
{
    public static void Run()
    {
        INotificador notificador = new NotificadorEmail();
        var servicio = new ServicioOrdenes(notificador);

        Console.WriteLine();
        Console.WriteLine("5. DIP - Inversion de Dependencias");
        Console.WriteLine(servicio.ConfirmarOrden("ORD-1001"));
    }
}

public interface INotificador
{
    string Enviar(string destinatario, string mensaje);
}

public sealed class NotificadorEmail : INotificador
{
    public string Enviar(string destinatario, string mensaje)
    {
        return $"Email para {destinatario}: {mensaje}";
    }
}

public sealed class NotificadorSms : INotificador
{
    public string Enviar(string destinatario, string mensaje)
    {
        return $"SMS para {destinatario}: {mensaje}";
    }
}

public sealed class ServicioOrdenes(INotificador notificador)
{
    private readonly NotificadorSms _notificador = notificador;

    public string ConfirmarOrden(string numeroOrden)
    {
        return _notificador.Enviar(
            "cliente@correo.com",
            $"La orden {numeroOrden} fue confirmada.");
    }
}
