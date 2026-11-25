namespace RetailRocket.Domain.Entities.Shop;

public class OrderItem
{
    public Guid order_id { get; private set; }
    public Guid product_id { get; private set; }
    public Order? order { get; private set; }
    public Product? product { get; private set; }
    public uint quantity { get; private set; }
    public decimal price { get; private set; }

    public OrderItem(Guid order_id, Guid product_id, uint quantity, decimal price)
    {
        this.order_id = order_id;
        this.product_id = product_id;
        this.quantity = quantity;
        this.price = price;
    }
}