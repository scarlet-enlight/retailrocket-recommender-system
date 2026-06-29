using RetailRocket.Domain.Entities.Shop;

namespace RetailRocket.Application.DTOs.Request.Shop;

public class CreateCartDto
{
    public User? User { get; private set; }
    public Product? Product { get; private set; }
    public uint Quantity { get; private set; }
}