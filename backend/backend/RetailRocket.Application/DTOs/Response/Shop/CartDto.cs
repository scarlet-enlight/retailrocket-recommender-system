namespace RetailRocket.Application.DTOs.Response.Shop;

public class CartDto
{
    public Guid CartId { get; set; }
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public uint Quantity { get; set; }
    public DateTime AddedAt { get; set; }
}