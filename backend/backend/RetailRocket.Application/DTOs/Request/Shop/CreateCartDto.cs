namespace RetailRocket.Application.DTOs.Request.Shop;

public class CreateCartDto
{
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public uint Quantity { get; set; }
}