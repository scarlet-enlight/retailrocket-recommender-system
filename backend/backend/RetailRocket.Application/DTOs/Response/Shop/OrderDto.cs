using RetailRocket.Domain.Entities.Shop;

namespace RetailRocket.Application.DTOs.Response.Shop;

public class OrderDto
{
    public Guid OrderId { get; }
    public User? User { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public decimal Total { get; private set; }
}