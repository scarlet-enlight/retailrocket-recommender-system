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

    public async Task<Order?> GetByIdAsync(Guid id) =>
        await _dbContext.Orders
            .Include(o => o.User)
            .Include(o => o.Total)
            .FirstOrDefaultAsync();

    public async Task<IEnumerable<Order>> GetByUserIdAsync(Guid userId) =>
        await _dbContext.Orders
            .Include(o => o.User)
            .Include(o => o.Total)
            .Where(o => o.UserId == userId)
            .ToListAsync();

    public async Task AddAsync(Order order)
    {
        await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var order = await GetByIdAsync(id);
        if (order is null) return;
        _dbContext.Orders.Remove(order);
        await _dbContext.SaveChangesAsync();
    }
}