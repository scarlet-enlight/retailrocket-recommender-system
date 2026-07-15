using RetailRocket.Domain.Entities.Historical;
namespace RetailRocket.Domain.Entities.Shop;

public class Product
{
    public Guid ProductId { get; }
    public int ItemId { get; private set; }
    public Item? Item { get; set; }
    public string? Name { get; private set; }
    public decimal? Price { get; private set; }
    public int CategoryId { get; private set; }
    public Category? Category { get; set; }

    public Product(int itemId, string? name, decimal? price, int categoryId)
    {
        ItemId = itemId;
        Name = name;
        Price = price;
        CategoryId = categoryId;
    }
    
    public void UpdateItem(int itemId) => ItemId = itemId;
    public void UpdateName(string? name) => Name = name;
    public void UpdatePrice(decimal? price) => Price = price;
    public void UpdateCategory(int categoryId) => CategoryId = categoryId;
}