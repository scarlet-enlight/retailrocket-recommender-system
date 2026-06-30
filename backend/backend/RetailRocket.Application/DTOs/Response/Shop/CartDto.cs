using RetailRocket.Domain.Entities.Shop;

namespace RetailRocket.Application.DTOs.Response.Shop;

public class CartDto
{
    public User? User { get; set; }
    public Product? Product { get; set; }
    public uint Quantity { get; set; }
}