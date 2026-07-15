using System.Numerics;

namespace RetailRocket.Domain.Entities.Historical;

public class Transaction
{
    public int TransactionId { get; }
    public int VisitorId { get; private set; }
    public Visitor? Visitor { get; private set; }
    public BigInteger Timestamp { get; }

    public Transaction(int visitorId, BigInteger timestamp)
    {
        VisitorId = visitorId;
        Timestamp = timestamp;
    }
}