namespace RetailRocket.Domain.Entities.Historical;

public class Visitor
{
    public int visitor_id { get; }

    public Visitor(int visitor_id)
    {
        this.visitor_id = visitor_id;
    }
}