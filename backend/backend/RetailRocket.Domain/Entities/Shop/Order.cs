namespace RetailRocket.Domain.Entities.Shop;

public class Order
{
    public int OrderId { get; }
    public int UserId { get; private set; }
    public User? User { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal Total { get; private set; }

    public Order(int userId, decimal total)
    {
        UserId = userId;
        Total = total;
    }
    
    public void UpdateTotal(decimal total) => Total = total;
    
    // needs updating in terms of total price
}