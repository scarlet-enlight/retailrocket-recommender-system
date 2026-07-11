namespace RetailRocket.Application.DTOs.Response.Shop;

public class CartDto
{
    public int CartId { get; set; }
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public uint Quantity { get; set; }
    public DateTime AddedAt { get; set; }
}