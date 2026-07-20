namespace RetailRocket.Application.DTOs.Request.Shop;

public record ProductRequestDto
{
    public int ItemId { get; set; }
    public string? Name { get; set; }
    public decimal? Price { get; set; }
    public int CategoryId { get; set; }
}