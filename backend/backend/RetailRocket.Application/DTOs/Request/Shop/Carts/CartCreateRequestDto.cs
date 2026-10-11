namespace RetailRocket.Application.DTOs.Request.Shop.Carts;

public record CartCreateRequestDto
{
    public Guid ProductId { get; set; }
    public uint Quantity { get; set; }
}