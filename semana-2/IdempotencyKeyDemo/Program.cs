using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var processedRequests = new ConcurrentDictionary<string, PaymentResponse>();

app.MapPost("/payments", (PaymentRequest request, HttpContext http) =>
{
    var idempotencyKey = http.Request.Headers["Idempotency-Key"].FirstOrDefault();

    if (string.IsNullOrWhiteSpace(idempotencyKey))
    {
        return Results.BadRequest(new
        {
            error = "Falta el encabezado Idempotency-Key."
        });
    }

    if (processedRequests.TryGetValue(idempotencyKey, out var cachedResponse))
    {
        return Results.Ok(new
        {
            message = "La misma solicitud ya fue procesada.",
            repeated = true,
            idempotencyKey,
            result = cachedResponse
        });
    }

    // Procesamiento 
    
    var response = new PaymentResponse
    {
        Id = Guid.NewGuid().ToString(),
        Amount = request.Amount,
        Currency = request.Currency,
        Status = "Processed",
        IdempotencyKey = idempotencyKey
    };

    if (!processedRequests.TryAdd(idempotencyKey, response))
    {
        return Results.Ok(new
        {
            message = "Otra petición concurrente ya había registrado esta clave.",
            repeated = true,
            idempotencyKey,
            result = processedRequests[idempotencyKey]
        });
    }

    return Results.Created($"/payments/{response.Id}", new
    {
        message = "Pago registrado correctamente.",
        repeated = false,
        idempotencyKey,
        result = response
    });
});

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();

public record PaymentRequest(decimal Amount, string Currency);

public class PaymentResponse
{
    public string Id { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string IdempotencyKey { get; set; } = string.Empty;
}
