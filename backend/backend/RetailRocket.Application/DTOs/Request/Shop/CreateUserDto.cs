namespace RetailRocket.Application.DTOs.Request.Shop;

public class CreateUserDto
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
}