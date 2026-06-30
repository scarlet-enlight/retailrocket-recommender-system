using RetailRocket.Application.Interfaces.Shop;
using RetailRocket.Domain.Entities.Shop;

namespace RetailRocket.Application.Services.Shop;

public class CartService
{
    private readonly ICartRepository _cartRepository;
    
    public CartService(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public async Task<Cart?> GetCartAsync(Guid id) =>
        await _cartRepository.GetByIdAsync(id);
    
    public async Task<IEnumerable<Cart>> GetCartsByUserAsync(Guid id) =>
        await _cartRepository.GetByUserIdAsync(id);
    
    public async Task AddCartAsync(Cart cart) =>
        await _cartRepository.AddAsync(cart);
    
    public async Task UpdateCartAsync(Cart cart) =>
        await _cartRepository.UpdateAsync(cart);

    public async Task DeleteCartAsync(Guid id) =>
        await _cartRepository.DeleteAsync(id);
}