namespace RetailRocket.Application.DTOs.Request.Shop;

public class CreateProductDto
{
    public Guid ItemId { get; set; }
    public string? Name { get; set; }
    public decimal? Price { get; set; }
    public Guid CategoryId { get; set; }
}