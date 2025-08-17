using Microsoft.EntityFrameworkCore;
using Shared.Entities;
using Shared.Contracts.Products;
using ProductService.Infrastructure;
using Mapster;
using ProductService.Interfaces;

namespace ProductService.Services;

public class AppProductService(ProductDbContext context) : IAppProductService
{
    private readonly ProductDbContext _context = context;

    public async Task<IEnumerable<ProductResponse>> GetAllProductsAsync()
    {
        var products = await _context.Products.ToListAsync();
        return products.Adapt<IEnumerable<ProductResponse>>();
    }

    public async Task<ProductResponse?> GetProductByIdAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        return product?.Adapt<ProductResponse>();
    }

    public async Task<ProductResponse> CreateProductAsync(SaveProductRequest request)
    {
        var product = request.Adapt<Product>();

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return product.Adapt<ProductResponse>();
    }

    public async Task<ProductResponse?> UpdateProductAsync(int id, SaveProductRequest request)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return null;

        request.Adapt(product);

        _context.Products.Update(product);
        await _context.SaveChangesAsync();

        return product.Adapt<ProductResponse>();
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return true;
    }
}
