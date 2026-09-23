namespace RetailRocket.Application.DTOs.Request.Shop;

public record CartRequestDto
{
    public Guid ProductId { get; set; }
    public uint Quantity { get; set; }
}