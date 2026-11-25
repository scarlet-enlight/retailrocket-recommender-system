namespace RetailRocket.Domain.Entities.Shop;

public class Order
{
    public Guid order_id { get; }
    public Guid user_id { get; private set; }
    public User? user { get; set; }
    public DateTime created_at { get; set; }
    public decimal total { get; private set; }

    public Order(Guid user_id, decimal total)
    {
        this.user_id = user_id;
        this.total = total;
    }
    
    public void UpdateTotal(decimal total) => this.total = total;
    
    // needs updating in terms of total price
}