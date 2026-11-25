using RetailRocket.Domain.Entities.Historical;
namespace RetailRocket.Domain.Entities.Shop;

public class Product
{
    public Guid product_id { get; }
    public int item_id { get; private set; }
    public Item item { get; set; }
    public string? name { get; private set; }
    public decimal? price { get; private set; }
    public int category_id { get; private set; }
    public Category category { get; set; }

    public Product(int item_id, string? name, decimal? price, int category_id)
    {
        this.item_id = item_id;
        this.name = name;
        this.price = price;
        this.category_id = category_id;
    }
    
    public void UpdateItem(int item_id) => this.item_id = item_id;
    public void UpdateName(string? name) => this.name = name;
    public void UpdatePrice(decimal? price) => this.price = price;
    public void UpdateCategory(int category_id) => this.category_id = category_id;
}