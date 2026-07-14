namespace RetailRocket.Application.DTOs.Request.Shop;

public class CreateCartDto
{
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public uint Quantity { get; set; }
}