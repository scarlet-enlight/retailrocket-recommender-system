namespace RetailRocket.Application.DTOs.Short.Shop;

public record UserShortDto()
{
    public Guid UserId { get; set; }
    public string? Username { get; set; }
}