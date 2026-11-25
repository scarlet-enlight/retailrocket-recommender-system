namespace RetailRocket.Domain.Entities.Shop;

public class Cart
{
    public Guid cart_id { get; }
    public Guid user_id { get; private set; }
    public User user { get; set; }
    public Guid product_id { get; private set; }
    public Product product { get; set; }
    public int quantity { get; private set; }
    public DateTime added_at { get; }

    public Cart(Guid user_id, Guid product_id, int quantity)
    {
        this.user_id = user_id;
        this.product_id = product_id;
        this.quantity = quantity;
    }

    // not sure about updating those values (needs to update also Product)
    public void UpdateProduct(Guid product_id) => this.product_id = product_id;
    public void UpdateQuantity(int quantity) => this.quantity = quantity;
}