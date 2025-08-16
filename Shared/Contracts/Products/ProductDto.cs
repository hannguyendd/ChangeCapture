namespace Shared.Contracts.Products;

public record ProductResponse(int Id, decimal Price)
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public record SaveProductRequest(string Name, string Description, decimal Price);


