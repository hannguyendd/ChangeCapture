using Microsoft.EntityFrameworkCore;
using Shared.Entities;

namespace ProductService.Infrastructure;

public class ProductDbContext(DbContextOptions<ProductDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
}