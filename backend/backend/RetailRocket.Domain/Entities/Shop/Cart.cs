namespace RetailRocket.Domain.Entities.Shop;

public class Cart
{
    public Guid CartId { get; }
    public Guid UserId { get; private set; }
    public User? User { get; set; }
    public Guid ProductId { get; private set; }
    public Product? Product { get; set; }
    public uint Quantity { get; private set; }
    public DateTime AddedAt { get; }

    public Cart(Guid userId, Guid productId, uint quantity)
    {
        UserId = userId;
        ProductId = productId;
        Quantity = quantity;
    }

    // not sure about updating those values
    public void UpdateProduct(Guid productId) => ProductId = productId;
    public void UpdateQuantity(uint quantity) => Quantity = quantity;
}