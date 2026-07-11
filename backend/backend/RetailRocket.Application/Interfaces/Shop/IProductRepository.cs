using RetailRocket.Domain.Entities.Shop;

namespace RetailRocket.Application.Interfaces.Shop;

public interface IProductRepository : IRepository<Product, int>
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByNameAsync(string productName);
    Task UpdateAsync(Product product);
}