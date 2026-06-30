namespace RetailRocket.Domain.Entities.Shop;

public class Cart
{
    public Guid CartId { get; }
    public Guid UserId { get; }
    public User? User { get; private set; }
    public Guid ProductId { get; }
    public Product? Product { get; private set; }
    public uint Quantity { get; private set; }
    public DateTime AddedAt { get; }

    public Cart(User? user, Product? product, uint quantity)
    {
        User = user;
        Product = product;
        Quantity = quantity;
    }

    // not sure about updating those values
    public void UpdateProduct(Product? product) => Product = product;
    public void UpdateQuantity(uint quantity) => Quantity = quantity;
}