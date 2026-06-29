using Microsoft.EntityFrameworkCore;
using RetailRocket.Application.Interfaces.Shop;
using RetailRocket.Domain.Entities.Shop;
using RetailRocket.Infrastructure.Persistence;

namespace RetailRocket.Infrastructure.Repositories.Shop;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _dbContext;

    public ProductRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Product?> GetByIdAsync(Guid id) => 
        await _dbContext.Products
            .Include(p => p.Name)
            .Include(p => p.Category)
            .Include(p => p.Price)
            .FirstOrDefaultAsync(p => p.ProductId == id);

    public async Task<IEnumerable<Product>> GetAllAsync() =>
        await _dbContext.Products
            .Include(p => p.Category)
            .ToListAsync();

    public async Task<Product?> GetByNameAsync(string productName) =>
        await _dbContext.Products
            .Include(p => p.Name)
            .Include(p => p.Category)
            .Include(p => p.Price)
            .FirstOrDefaultAsync(p => p.Name == productName);

    public async Task AddAsync(Product product)
    {
        await _dbContext.Products.AddAsync(product);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        _dbContext.Products.Update(product);
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task DeleteAsync(Guid id)
    {
        var product = await GetByIdAsync(id);
        if (product is null) return;
        _dbContext.Products.Remove(product);
        await _dbContext.SaveChangesAsync();
    }
}