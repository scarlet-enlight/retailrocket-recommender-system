namespace RetailRocket.Application.DTOs.Request.Shop;

public class CreateOrderDto
{
    public Guid UserId { get; set; }
    public decimal Total { get; set; }
}