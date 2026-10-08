namespace SseDemo.Models;

// Cada evento lleva el estado completo. El navegador puede reemplazar su estado local.
public sealed record Pedido(int Id, string Estado, long Version, DateTimeOffset ActualizadoEn);
public sealed record CambiarEstadoRequest(string? Estado);
