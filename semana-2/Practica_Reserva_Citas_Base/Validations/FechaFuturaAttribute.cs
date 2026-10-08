using System.ComponentModel.DataAnnotations;

namespace PracticaReservaCitasBase.Validations;

public class FechaFuturaAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(
        object? value,
        ValidationContext validationContext)
    {
        // TODO: La fecha y hora deben ser posteriores al momento actual.
        // Cuando el valor sea valido, devolver ValidationResult.Success.
        // Cuando sea invalido, devolver un ValidationResult con un mensaje apropiado.

        return ValidationResult.Success;
    }
}
