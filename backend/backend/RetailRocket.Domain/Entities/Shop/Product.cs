using RetailRocket.Domain.Entities.Historical;
namespace RetailRocket.Domain.Entities.Shop;

public class Product
{
    public Guid ProductId { get; }
    public Guid ItemId { get; private set; }
    public Item? Item { get; set; }
    public string? Name { get; private set; }
    public decimal? Price { get; private set; }
    public Guid CategoryId { get; private set; }
    public Category? Category { get; set; }

    public Product(Guid itemId, string? name, decimal? price, Guid categoryId)
    {
        ItemId = itemId;
        Name = name;
        Price = price;
        CategoryId = categoryId;
    }

    public void UpdateItem(Guid itemId) => ItemId = itemId;
    public void UpdateName(string? name) => Name = name;
    public void UpdatePrice(decimal? price) => Price = price;
    public void UpdateCategory(Guid categoryId) => CategoryId = categoryId;
}