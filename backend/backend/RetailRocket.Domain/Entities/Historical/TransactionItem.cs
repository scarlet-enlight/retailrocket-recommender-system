namespace RetailRocket.Domain.Entities.Historical;

public class TransactionItem
{
    public int TransactionId { get; private set; }
    public int ItemId { get; private set; }
    public Transaction?  Transaction { get; private set; }
    public Item? Item { get; private set; }

    public TransactionItem(int transactionId, int itemId)
    {
        TransactionId = transactionId;
        ItemId = itemId;
    }
}