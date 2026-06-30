using RetailRocket.Domain.Entities.Historical;
namespace RetailRocket.Domain.Entities.Shop;

public class Product
{
    public Guid ProductId { get; }
    public Guid ItemId { get; }
    public Item? Item { get; private set; }
    public string? Name { get; private set; }
    public decimal? Price { get; private set; }
    public Guid CategoryId { get; }
    public Category? Category { get; private set; }

    public Product(Item? item, string? name, decimal? price, Category? category)
    {
        Item = item;
        Name = name;
        Price = price;
        Category = category;
    }

    public void UpdateItem(Item? item) =>  Item = item;
    public void UpdateName(string? name) => Name = name;
    public void UpdatePrice(decimal? price) => Price = price;
    public void UpdateCategory(Category? category) => Category = category;
}