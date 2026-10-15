using System.ComponentModel.DataAnnotations;

namespace EFCore_Semana6.Endpoints;

public static class RequestValidator
{
    public static Dictionary<string, string[]> Validar<T>(T request)
    {
        var resultados = new List<ValidationResult>();
        var contexto = new ValidationContext(request!);

        Validator.TryValidateObject(request!, contexto, resultados, validateAllProperties: true);

        return resultados
            .SelectMany(resultado => resultado.MemberNames.DefaultIfEmpty(string.Empty)
                .Select(miembro => new { miembro, resultado.ErrorMessage }))
            .GroupBy(error => error.miembro)
            .ToDictionary(
                grupo => grupo.Key,
                grupo => grupo.Select(error => error.ErrorMessage ?? "Valor inválido.").ToArray());
    }
}
