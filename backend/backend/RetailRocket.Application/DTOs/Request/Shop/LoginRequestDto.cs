namespace RetailRocket.Application.DTOs.Request.Shop;

public record LoginRequestDto
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}