using RetailRocket.Domain.Entities.Shop;
using RetailRocket.Domain.Entities.Historical;

namespace RetailRocket.Application.DTOs.Request.Shop;

public class CreateProductDto
{
    public Item? Item { get; private set; }
    public string? Name { get; private set; }
    public decimal? Price { get; private set; }
    public Category? Category { get; private set; }
}