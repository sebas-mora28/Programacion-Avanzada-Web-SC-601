namespace PracticaReservaCitasBase.Models;

public class Cita
{
    // TODO 1: Impedir que Id participe en Model Binding.
    public int Id { get; set; }

    // TODO 2: Agregar las validaciones indicadas en la especificacion.
    public string NombreCliente { get; set; } = string.Empty;

    // TODO 3: Agregar las validaciones indicadas en la especificacion.
    public string Correo { get; set; } = string.Empty;

    // TODO 4: Aplicar la validacion personalizada FechaFutura.
    public DateTime FechaHora { get; set; }

    // TODO 5: Marcar el tipo de servicio como obligatorio.
    public string TipoServicio { get; set; } = string.Empty;

    // TODO 6: El motivo es opcional, pero debe tener un maximo de 200 caracteres.
    public string? Motivo { get; set; }

    // TODO 7: Impedir que Estado participe en Model Binding.
    public string Estado { get; set; } = string.Empty;
}
