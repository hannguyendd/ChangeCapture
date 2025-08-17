using Shared.Contracts.Products;

namespace ProductService.Interfaces;

public interface IAppProductService
{
    Task<IEnumerable<ProductResponse>> GetAllProductsAsync();
    Task<ProductResponse?> GetProductByIdAsync(int id);
    Task<ProductResponse> CreateProductAsync(SaveProductRequest request);
    Task<ProductResponse?> UpdateProductAsync(int id, SaveProductRequest request);
    Task<bool> DeleteProductAsync(int id);
}