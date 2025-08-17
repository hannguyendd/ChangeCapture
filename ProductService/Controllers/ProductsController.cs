using Microsoft.AspNetCore.Mvc;
using Shared.Contracts.Products;
using ProductService.Interfaces;

namespace ProductService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IAppProductService productService) : ControllerBase
{
    private readonly IAppProductService _productService = productService;

    /// <summary>
    /// Get all products
    /// </summary>
    /// <returns>List of products</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetAllProducts()
    {
        var products = await _productService.GetAllProductsAsync();
        return Ok(products);
    }

    /// <summary>
    /// Get a product by ID
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>Product details</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductResponse>> GetProduct(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);

        if (product == null)
            return NotFound($"Product with ID {id} not found.");

        return Ok(product);
    }

    /// <summary>
    /// Create a new product
    /// </summary>
    /// <param name="request">Product creation request</param>
    /// <returns>Created product</returns>
    [HttpPost]
    public async Task<ActionResult<ProductResponse>> CreateProduct(SaveProductRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var product = await _productService.CreateProductAsync(request);

        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    /// <summary>
    /// Update an existing product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="request">Product update request</param>
    /// <returns>Updated product</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<ProductResponse>> UpdateProduct(int id, SaveProductRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var product = await _productService.UpdateProductAsync(id, request);

        if (product == null)
            return NotFound($"Product with ID {id} not found.");

        return Ok(product);
    }

    /// <summary>
    /// Delete a product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>Confirmation of deletion</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var deleted = await _productService.DeleteProductAsync(id);

        if (!deleted)
            return NotFound($"Product with ID {id} not found.");

        return NoContent();
    }
}
