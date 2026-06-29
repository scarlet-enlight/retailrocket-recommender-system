using RetailRocket.Domain.Entities.Historical;

namespace RetailRocket.Application.DTOs.Response.Shop;

public class ProductDto
{
    public Guid ProductId { get; }
    public Item? Item { get; private set; }
    public string? Name { get; private set; }
    public decimal? Price { get; private set; }
    public Category? Category { get; private set; }
}