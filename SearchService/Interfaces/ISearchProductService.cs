using Shared.Contracts.Products;

namespace SearchService.Interfaces;

public interface ISearchProductService
{
    Task<IEnumerable<ProductResponse>> SearchProductsAsync(string searchTerm, int page = 1, int pageSize = 10);
    Task<ProductResponse?> GetProductByIdAsync(int id);
    Task<IEnumerable<ProductResponse>> GetAllProductsAsync(int page = 1, int pageSize = 10);
    Task IndexProductAsync(ProductChangeEvent productEvent);
    Task DeleteProductAsync(int id);
}