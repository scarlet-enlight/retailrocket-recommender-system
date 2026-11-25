using System.Numerics;

namespace RetailRocket.Domain.Entities.Historical;

public class Transaction
{
    public int transaction_id { get; }
    public int visitor_id { get; private set; }
    public Visitor? visitor { get; private set; }
    public BigInteger ts { get; }

    public Transaction(int visitor_id, BigInteger ts)
    {
        this.visitor_id = visitor_id;
        this.ts = ts;
    }
}