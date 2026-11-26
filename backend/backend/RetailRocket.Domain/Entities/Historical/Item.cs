namespace RetailRocket.Domain.Entities.Historical;

public class Item
{
    public int item_id { get; }
    public int category_id { get; private set; }
    public Category? category { get; private set; }
    public bool available { get; private set;  }

    public Item(int item_id, int category_id, bool available)
    {
        this.item_id = item_id;
        this.category_id = category_id;
        this.available = available;
    }
}