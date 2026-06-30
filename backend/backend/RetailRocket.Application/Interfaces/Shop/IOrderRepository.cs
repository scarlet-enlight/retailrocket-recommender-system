using RetailRocket.Domain.Entities.Shop;

namespace RetailRocket.Application.Interfaces.Shop;

public interface IOrderRepository :  IRepository<Order, Guid>
{
    Task<IEnumerable<Order>> GetByUserIdAsync(Guid userId);
    Task UpdateTotal(decimal total);
}