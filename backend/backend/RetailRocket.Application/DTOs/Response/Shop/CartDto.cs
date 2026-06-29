using RetailRocket.Domain.Entities.Shop;

namespace RetailRocket.Application.DTOs.Response.Shop;

public class CartDto
{
    public User? User { get; private set; }
    public Product? Product { get; private set; }
    public uint Quantity { get; private set; }
}