namespace RetailRocket.Domain.Entities.Shop;

public class OrderItem
{
    public int OrderId { get; private set; }
    public int ProductId { get; private set; }
    public Order? Order { get; private set; }
    public Product? Product { get; private set; }
    public uint Quantity { get; private set; }
    public decimal Price { get; private set; }

    public OrderItem(int orderId, int productId, uint quantity, decimal price)
    {
        OrderId = orderId;
        ProductId = productId;
        Quantity = quantity;
        Price = price;
    }
}