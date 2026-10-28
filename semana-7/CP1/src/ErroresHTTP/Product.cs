namespace Practica.DisenoHttp;

public sealed record Product(int Id, string Name, decimal Price);
public sealed record UpdateProductRequest(string Name, decimal Price);
