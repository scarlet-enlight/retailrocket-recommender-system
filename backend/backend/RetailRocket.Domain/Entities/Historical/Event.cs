using System.Numerics;
using System.Runtime.InteropServices.JavaScript;
using RetailRocket.Domain.Entities.Enums;
    
namespace RetailRocket.Domain.Entities.Historical;

public class Event
{
    public Guid EventId { get; }
    public Guid VisitorId { get; private set; }
    public Visitor? Visitor { get; private set; }
    public Guid ItemId { get; private set; }
    public Item? Item { get; private set; }
    public EventType? EventType { get; private set; }
    public BigInteger Timestamp { get; }
}