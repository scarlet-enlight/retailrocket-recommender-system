using System.Numerics;

namespace RetailRocket.Domain.Entities.Historical;

public class Transaction
{
    public int transaction_id { get; }
    public int visitor_id { get; private set; }
    public Visitor? visitor { get; private set; }
    public BigInteger ts { get; }

    public Transaction(int transaction_id, int visitor_id, BigInteger ts)
    {
        this.transaction_id = transaction_id;
        this.visitor_id = visitor_id;
        this.ts = ts;
    }
}