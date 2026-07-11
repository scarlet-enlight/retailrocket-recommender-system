namespace RetailRocket.Domain.Entities.Historical;

public class Item
{
    public int ItemId { get; }
    public int CategoryId { get; private set; }
    public Category? Category { get; private set; }
    public bool IsAvailable { get; private set;  }

    public Item(int categoryId, bool isAvailable)
    {
        CategoryId = categoryId;
        IsAvailable = isAvailable;
    }
}