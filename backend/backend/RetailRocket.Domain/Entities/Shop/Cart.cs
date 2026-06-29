namespace RetailRocket.Domain.Entities.Shop;

public class Cart
{
    public Guid CartId { get; }
    public Guid UserId { get; private set; }
    public User? User { get; private set; }
    public Guid ProductId { get; private set; }
    public Product? Product { get; private set; }
    public uint Quantity { get; private set; }
    public DateTime AddedAt { get; }
}