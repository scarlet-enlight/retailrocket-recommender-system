namespace RetailRocket.Domain.Entities.Historical;

public class Item
{
    public Guid ItemId { get; }
    public Guid CategoryId { get; private set; }
    public Category? Category { get; private set; }
    public bool IsAvailable { get; private set;  }
}