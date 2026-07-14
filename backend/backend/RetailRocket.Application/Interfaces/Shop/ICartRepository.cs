using RetailRocket.Domain.Entities.Shop;

namespace RetailRocket.Application.Interfaces.Shop;

public interface ICartRepository : IRepository<Cart, Guid>
{
    Task<IEnumerable<Cart>> GetByUserIdAsync(Guid userId);
    Task UpdateAsync(Cart cart);
}