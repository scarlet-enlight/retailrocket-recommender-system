namespace RetailRocket.Application.DTOs.Response.Shop;

public class CartDto
{
    public Guid CartId { get; set; }
    public string? ProductName { get; set; }
    public decimal? ProductPrice { get; set; }
    public uint Quantity { get; set; }
}