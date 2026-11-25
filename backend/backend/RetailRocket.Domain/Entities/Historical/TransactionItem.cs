namespace RetailRocket.Domain.Entities.Historical;

public class TransactionItem
{
    public int transaction_id { get; private set; }
    public int item_id { get; private set; }
    public Transaction? transaction { get; private set; }
    public Item? item { get; private set; }

    public TransactionItem(int transaction_id, int item_id)
    {
        this.transaction_id = transaction_id;
        this.item_id = item_id;
    }
}