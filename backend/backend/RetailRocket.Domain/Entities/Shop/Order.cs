namespace RetailRocket.Domain.Entities.Shop;

public class Order
{
    public Guid OrderId { get; }
    public Guid UserId { get; private set; }
    public User? User { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public decimal Total { get; private set; }
}