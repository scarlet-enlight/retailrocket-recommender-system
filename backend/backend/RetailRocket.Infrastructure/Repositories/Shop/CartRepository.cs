using Microsoft.EntityFrameworkCore;
using RetailRocket.Application.Interfaces.Shop;
using RetailRocket.Domain.Entities.Shop;
using RetailRocket.Infrastructure.Persistence;

namespace RetailRocket.Infrastructure.Repositories.Shop;

public class CartRepository : ICartRepository
{
    private readonly AppDbContext _dbContext;
    
    public CartRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Cart?> GetByIdAsync(int id) => 
        await  _dbContext.Carts
            .Include(c => c.User)
            .Include(c => c.Product)
            .FirstOrDefaultAsync(c => c.CartId == id);
    
    public async Task<IEnumerable<Cart>> GetByUserIdAsync(int id) =>
        await  _dbContext.Carts
            .Include(c => c.User)
            .Include(c => c.Product)
            .Where(c => c.UserId == id)
            .ToListAsync();

    public async Task AddAsync(Cart cart)
    {
        await _dbContext.Carts.AddAsync(cart);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Cart cart)
    {
        _dbContext.Carts.Update(cart);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var cart = await GetByIdAsync(id);
        if (cart is null) return;
        _dbContext.Carts.Remove(cart);
        await _dbContext.SaveChangesAsync();
    }
}