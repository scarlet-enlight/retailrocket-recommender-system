namespace RetailRocket.Application.DTOs.Request.Shop.Users;

public record UserUpdateRequestDto
{
    public string? Username { get; set; }
    public string? Email { get; set; }
}