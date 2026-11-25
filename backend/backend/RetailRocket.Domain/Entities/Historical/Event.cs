using System.Numerics;

namespace RetailRocket.Domain.Entities.Historical;

public class Event
{
    public int event_id { get; }
    public int? visitor_id { get; set; }
    public Visitor? visitor { get; set; }
    public int? item_id { get; set; }
    public Item? item { get; set; }
    public string? event_type { get; private set; }
    public BigInteger ts { get; }

    public Event(int? visitor_id, int? item_id, string event_type, BigInteger ts)
    {
        this.visitor_id = visitor_id;
        this.item_id = item_id;
        this.event_type = event_type;
        this.ts = ts;
    }
}