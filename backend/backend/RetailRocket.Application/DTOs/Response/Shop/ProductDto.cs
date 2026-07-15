namespace RetailRocket.Application.DTOs.Response.Shop;

public class ProductDto
{
    public Guid ProductId { get; set; }
    public string? Name { get; set; }
    public decimal? Price { get; set; }
    public string? CategoryName { get; set; }
    public bool IsAvailable { get; set; }
}