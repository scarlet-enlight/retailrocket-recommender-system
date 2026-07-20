using RetailRocket.Application.DTOs.Short.Historical;

namespace RetailRocket.Application.DTOs.Response.Shop;

public record ProductResponseDto
{
    public Guid ProductId { get; set; }
    public ItemShortDto Item { get; set; }
    public string? Name { get; set; }
    public decimal? Price { get; set; }
}