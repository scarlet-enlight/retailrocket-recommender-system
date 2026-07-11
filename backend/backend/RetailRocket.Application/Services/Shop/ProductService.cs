using RetailRocket.Application.Interfaces.Shop;
using RetailRocket.Domain.Entities.Shop;

namespace RetailRocket.Application.Services.Shop;

public class ProductService
{
    private readonly IProductRepository _productRepository;
    
    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }
    
    public async Task<IEnumerable<Product>> GetAllProductsAsync() => 
        await _productRepository.GetAllAsync();
    
    public async Task<Product?> GetProductAsync(int id) => 
        await _productRepository.GetByIdAsync(id);
    
    public async Task<Product?> GetProductByNameAsync(string productName) =>
        await _productRepository.GetByNameAsync(productName);
    
    public async Task CreateProductAsync(Product product) =>
        await _productRepository.AddAsync(product);
    
    public async Task UpdateProductAsync(Product product) =>
        await _productRepository.UpdateAsync(product);
    
    public async Task DeleteProductAsync(int id) =>
        await _productRepository.DeleteAsync(id);
}