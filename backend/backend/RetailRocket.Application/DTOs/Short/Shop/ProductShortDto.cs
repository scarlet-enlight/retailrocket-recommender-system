namespace RetailRocket.Application.DTOs.Short.Shop;

public record ProductShortDto()
{
    public string? Name { get; set; }
    public decimal? Price { get; set;  }
}