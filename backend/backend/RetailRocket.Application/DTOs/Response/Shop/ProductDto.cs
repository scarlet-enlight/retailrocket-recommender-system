namespace RetailRocket.Application.DTOs.Response.Shop;

public class ProductDto
{
    public Guid ProductId { get; set; }
    public int ItemId { get; set; }
    public string? Name { get; set; }
    public decimal? Price { get; set; }
    public int CategoryId { get; set; }
}