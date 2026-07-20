using RetailRocket.Application.DTOs.Short.Shop;

namespace RetailRocket.Application.DTOs.Response.Shop;

public record CartResponseDto
{
    public Guid CartId { get; set; }
    public UserShortDto User { get; set; }
    public ProductShortDto Product { get; set; }
    public uint Quantity { get; set; }
}