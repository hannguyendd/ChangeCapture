using Microsoft.AspNetCore.Mvc;
using Shared.Contracts.Products;
using SearchService.Services;
using SearchService.Interfaces;

namespace SearchService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController(ISearchProductService searchService) : ControllerBase
{
    private readonly ISearchProductService _searchService = searchService;

    /// <summary>
    /// Search products by term (name, description)
    /// </summary>
    /// <param name="q">Search query</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <returns>List of matching products</returns>
    [HttpGet("products")]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> SearchProducts(
        [FromQuery] string? q = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var products = await _searchService.SearchProductsAsync(q ?? string.Empty, page, pageSize);
        return Ok(products);
    }

    /// <summary>
    /// Get all products with pagination
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <returns>List of all products</returns>
    [HttpGet("products/all")]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetAllProducts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var products = await _searchService.GetAllProductsAsync(page, pageSize);
        return Ok(products);
    }

    /// <summary>
    /// Get a specific product by ID from search index
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>Product details</returns>
    [HttpGet("products/{id}")]
    public async Task<ActionResult<ProductResponse>> GetProduct(int id)
    {
        var product = await _searchService.GetProductByIdAsync(id);

        if (product == null)
            return NotFound($"Product with ID {id} not found in search index.");

        return Ok(product);
    }
}
