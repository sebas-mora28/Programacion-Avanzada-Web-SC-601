using Practica.DisenoHttp;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ProductStore>();
var app = builder.Build();

// Esta API funciona, pero su contrato HTTP requiere una revisión.
// Analice las solicitudes, las respuestas y los efectos de cada operación.

app.MapGet("/products", (ProductStore store) =>
{
    return Results.Ok(store.GetAll());
});

app.MapGet("/products/{id:int}", (int id, ProductStore store) =>
{
    var product = store.GetById(id);
    if (product is null)
        return Results.Ok(new { error = "Product not found" });

    return Results.Ok(product);
});

app.MapGet("/products/create", (string name, decimal price, ProductStore store) =>
{
    var product = store.Create(name, price);
    return Results.Ok(product);
});

app.MapPost("/products/{id:int}/update", (int id, UpdateProductRequest request, ProductStore store) =>
{
    var product = store.IncreasePriceAndRename(id, request.Name, request.Price);
    if (product is null)
        return Results.Ok(new { error = "Product not found" });

    return Results.Ok(product);
});

app.MapGet("/products/{id:int}/delete", (int id, ProductStore store) =>
{
    var deleted = store.Delete(id);
    if (!deleted)
        return Results.Ok(new { error = "Product not found" });

    return Results.Ok(new { message = "Product deleted" });
});

app.Run();
