namespace RetailRocket.Application.DTOs.Response.Shop.Orders;

public record OrderResponseDto
{
    public Guid OrderId { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal Total { get; set; }
}