namespace RetailRocket.Application.DTOs.Response.Shop;

public class OrderDto
{
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal Total { get; set; }
}