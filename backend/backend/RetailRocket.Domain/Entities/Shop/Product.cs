using RetailRocket.Domain.Entities.Historical;
namespace RetailRocket.Domain.Entities.Shop;

public class Product
{
    public Guid ProductId { get; }
    public Guid ItemId { get; private set; }
    public Item? Item { get; private set; }
    public string? Name { get; private set; }
    public decimal? Price { get; private set; }
    public Guid CategoryId { get; private set; }
    public Category? Category { get; private set; }
}