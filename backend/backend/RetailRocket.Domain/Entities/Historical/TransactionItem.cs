namespace RetailRocket.Domain.Entities.Historical;

public class TransactionItem
{
    public Guid TransactionId { get; private set; }
    public Guid ItemId { get; private set; }
    public Transaction?  Transaction { get; private set; }
    public Item? Item { get; private set; }
}