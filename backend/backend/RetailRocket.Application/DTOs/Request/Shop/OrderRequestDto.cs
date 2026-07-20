namespace RetailRocket.Application.DTOs.Request.Shop;

public record OrderRequestDto
{
    public Guid UserId { get; set; }
    public decimal Total { get; set; }
}