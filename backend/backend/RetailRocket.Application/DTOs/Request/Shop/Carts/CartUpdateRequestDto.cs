namespace RetailRocket.Application.DTOs.Request.Shop.Carts;

public record CartUpdateRequestDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}