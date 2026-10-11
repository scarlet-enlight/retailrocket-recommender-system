namespace RetailRocket.Application.DTOs.Request.Shop.Carts;

public record CartUpdateRequestDto
{
    public Guid ProductId { get; set; }
    public uint Quantity { get; set; }
}