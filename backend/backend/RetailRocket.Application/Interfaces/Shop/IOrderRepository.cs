using RetailRocket.Domain.Entities.Shop;

namespace RetailRocket.Application.Interfaces.Shop;

public interface IOrderRepository :  IRepository<Order, int>
{
    Task<IEnumerable<Order>> GetByUserIdAsync(int userId);
    Task UpdateTotal(int id, decimal total);
}