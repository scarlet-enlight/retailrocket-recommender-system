namespace RetailRocket.Application.DTOs.Request.Shop;

public record CartRequestDto
{
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public uint Quantity { get; set; }
}