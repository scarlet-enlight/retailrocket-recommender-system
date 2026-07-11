namespace RetailRocket.Domain.Entities.Shop;

public class Cart
{
    public int CartId { get; }
    public int UserId { get; private set; }
    public User? User { get; set; }
    public int ProductId { get; private set; }
    public Product? Product { get; set; }
    public uint Quantity { get; private set; }
    public DateTime AddedAt { get; }

    public Cart(int userId, int productId, uint quantity)
    {
        UserId = userId;
        ProductId = productId;
        Quantity = quantity;
    }

    // not sure about updating those values
    public void UpdateProduct(int productId) => ProductId = productId;
    public void UpdateQuantity(uint quantity) => Quantity = quantity;
}