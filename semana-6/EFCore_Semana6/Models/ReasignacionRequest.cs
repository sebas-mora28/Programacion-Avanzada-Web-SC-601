using System.ComponentModel.DataAnnotations;

namespace EFCore_Semana6.Models;

public class ReasignacionRequest
{
    [Range(1, int.MaxValue)]
    public int UsuarioOrigenId { get; set; }

    [Range(1, int.MaxValue)]
    public int UsuarioDestinoId { get; set; }
}
