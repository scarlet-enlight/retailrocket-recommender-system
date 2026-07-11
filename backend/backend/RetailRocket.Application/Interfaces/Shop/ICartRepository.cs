using RetailRocket.Domain.Entities.Shop;

namespace RetailRocket.Application.Interfaces.Shop;

public interface ICartRepository : IRepository<Cart, int>
{
    Task<IEnumerable<Cart>> GetByUserIdAsync(int userId);
    Task UpdateAsync(Cart cart);
}