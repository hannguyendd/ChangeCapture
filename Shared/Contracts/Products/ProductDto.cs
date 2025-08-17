using System.ComponentModel.DataAnnotations;

namespace Shared.Contracts.Products;

public record BaseProductDto(string Name, string Description, decimal Price);

public record ProductResponse(int Id, string Name, string Description, decimal Price) : BaseProductDto(Name, Description, Price);

public record SaveProductRequest(
    [Required, StringLength(100, MinimumLength = 1)] string Name,
    [Required, StringLength(500, MinimumLength = 1)] string Description,
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")] decimal Price
) : BaseProductDto(Name, Description, Price);

public record ProductChangeEvent(int Id, string Name, string Description, decimal Price) : BaseProductDto(Name, Description, Price);

