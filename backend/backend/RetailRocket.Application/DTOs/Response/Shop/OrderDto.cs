namespace RetailRocket.Application.DTOs.Response.Shop;

public class OrderDto
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal Total { get; set; }
}