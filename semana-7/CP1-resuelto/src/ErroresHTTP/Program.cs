using Practica.DisenoHttp;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ProductStore>();
var app = builder.Build();

app.MapGet("/products", (ProductStore store) =>
{
    return Results.Ok(store.GetAll());
});

app.MapGet("/products/{id:int}", (int id, ProductStore store) =>
{
    var product = store.GetById(id);
    if (product is null)
        return Results.NotFound(new { error = "Product not found" });

    return Results.Ok(product);
});

app.MapPost("/products", (CreateProductRequest request, ProductStore store) =>
{
    if (string.IsNullOrWhiteSpace(request.Name) || request.Price < 0)
        return Results.BadRequest(new { error = "El nombre es obligatorio y el precio no puede ser negativo." });

    var product = store.Create(request.Name, request.Price);
    return Results.Created($"/products/{product.Id}", product);
});

app.MapPut("/products/{id:int}", (int id, UpdateProductRequest request, ProductStore store) =>
{
    if (string.IsNullOrWhiteSpace(request.Name) || request.Price < 0)
        return Results.BadRequest(new { error = "El nombre es obligatorio y el precio no puede ser negativo." });

    // Reemplazar los valores hace que repetir el mismo PUT sea idempotente.
    var product = store.Replace(id, request.Name, request.Price);
    if (product is null)
        return Results.NotFound(new { error = "Product not found" });

    return Results.Ok(product);
});

app.MapDelete("/products/{id:int}", (int id, ProductStore store) =>
{
    var deleted = store.Delete(id);
    if (!deleted)
        return Results.NotFound(new { error = "Product not found" });

    return Results.NoContent();
});

app.Run();
