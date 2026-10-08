using System.ComponentModel.DataAnnotations;

namespace Semana2RazorModelBinding.Validation;

public class PrecioPermitidoAttribute : ValidationAttribute
{
    private readonly decimal _minimo;
    private readonly decimal _maximo;

    public PrecioPermitidoAttribute(double minimo, double maximo)
    {
        _minimo = (decimal)minimo;
        _maximo = (decimal)maximo;
    }

    protected override ValidationResult? IsValid(
        object? value,
        ValidationContext validationContext)
    {
        if (value is not decimal precio)
        {
            return new ValidationResult("El precio debe ser un número válido");
        }

        if (precio < _minimo || precio > _maximo)
        {
            return new ValidationResult(
                $"El precio debe estar entre {_minimo:0.00} y {_maximo:0.00}");
        }

        return ValidationResult.Success;
    }
}
