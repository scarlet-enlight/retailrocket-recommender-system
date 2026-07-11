namespace RetailRocket.Application.DTOs.Request.Shop;

public class CreateOrderDto
{
    public int UserId { get; set; }
    public decimal Total { get; set; }
}