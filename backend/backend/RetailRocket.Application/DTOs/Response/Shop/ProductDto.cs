using RetailRocket.Domain.Entities.Historical;

namespace RetailRocket.Application.DTOs.Response.Shop;

public class ProductDto
{
    public Guid ProductId { get; set; }
    public Item? Item { get; set; }
    public string? Name { get; set; }
    public decimal? Price { get; set; }
    public Category? Category { get; set; }
}