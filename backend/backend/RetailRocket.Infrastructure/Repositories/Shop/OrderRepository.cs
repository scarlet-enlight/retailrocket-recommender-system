using Microsoft.EntityFrameworkCore;
using RetailRocket.Application.Interfaces.Shop;
using RetailRocket.Domain.Entities.Shop;
using RetailRocket.Infrastructure.Persistence;

namespace RetailRocket.Infrastructure.Repositories.Shop;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _dbContext;

    public OrderRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Order?> GetByIdAsync(int id) =>
        await _dbContext.Orders
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.OrderId == id);

    public async Task<IEnumerable<Order>> GetByUserIdAsync(int userId) =>
        await _dbContext.Orders
            .Include(o => o.User)
            .Where(o => o.UserId == userId)
            .ToListAsync();

    public async Task AddAsync(Order order)
    {
        await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();
    }
    
    // For total price update only (WIP)
    public async Task UpdateTotal(int id, decimal total)
    {
        var order = await GetByIdAsync(id);
        if (order is null) return;
        order.UpdateTotal(total);
        _dbContext.Orders.Update(order);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var order = await GetByIdAsync(id);
        if (order is null) return;
        _dbContext.Orders.Remove(order);
        await _dbContext.SaveChangesAsync();
    }
}