using System.Numerics;
using RetailRocket.Domain.Entities.Enums;
    
namespace RetailRocket.Domain.Entities.Historical;

public class Event
{
    public int EventId { get; }
    public int? VisitorId { get; set; }
    public Visitor? Visitor { get; set; }
    public int? ItemId { get; set; }
    public Item? Item { get; set; }
    public string? EventType { get; private set; }
    public long Timestamp { get; }

    public Event(int? visitorId, int? itemId, string eventType, long timestamp)
    {
        VisitorId = visitorId;
        ItemId = itemId;
        EventType = eventType;
        Timestamp = timestamp;
    }
}