namespace RetailRocket.Application.DTOs.Response.Shop;

public record OrderResponseDto
{
    public Guid OrderId { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal Total { get; set; }
}