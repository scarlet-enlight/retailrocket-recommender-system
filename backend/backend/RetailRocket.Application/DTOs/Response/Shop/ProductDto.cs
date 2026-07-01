namespace RetailRocket.Application.DTOs.Response.Shop;

public class ProductDto
{
    public Guid ProductId { get; set; }
    public Guid ItemId { get; set; }
    public string? Name { get; set; }
    public decimal? Price { get; set; }
    public Guid CategoryId { get; set; }
}