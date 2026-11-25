namespace RetailRocket.Application.DTOs.Request.Shop.Users;

public record RequestPasswordResetDto
{
    public string Email { get; set; }
}