using System.Numerics;

namespace RetailRocket.Domain.Entities.Historical;

public class Transaction
{
    public Guid TransactionId { get; }
    public Guid VisitorId { get; private set; }
    public Visitor? Visitor { get; private set; }
    public BigInteger Timestamp { get; }
}