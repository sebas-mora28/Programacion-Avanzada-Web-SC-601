namespace EFCore_Semana6.Models;

public record TareaDetalle(
    int Id,
    string Titulo,
    string? Descripcion,
    bool Completada,
    string Prioridad,
    DateTime FechaCreacion,
    int CategoriaId,
    string Categoria,
    int? ResponsableId,
    string? Responsable,
    string? EmailResponsable);
