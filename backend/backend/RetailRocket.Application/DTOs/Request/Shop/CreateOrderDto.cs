using RetailRocket.Domain.Entities.Shop;

namespace RetailRocket.Application.DTOs.Request.Shop;

public class CreateOrderDto
{
    public User? User { get; private set; }
    public decimal Total { get; private set; }
}