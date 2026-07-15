using RetailRocket.Application.Interfaces.Shop;
using RetailRocket.Domain.Entities.Shop;

namespace RetailRocket.Application.Services.Shop;

public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    
    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Order?> GetOrderAsync(Guid id) =>
        await _orderRepository.GetByIdAsync(id);
    
    public async Task<IEnumerable<Order>> GetOrdersByUserAsync(Guid id) =>
        await _orderRepository.GetByUserIdAsync(id);
    
    public async Task AddOrderAsync(Order order) => 
        await _orderRepository.AddAsync(order);

    public async Task DeleteOrderAsync(Guid id) =>
        await _orderRepository.DeleteAsync(id);
}