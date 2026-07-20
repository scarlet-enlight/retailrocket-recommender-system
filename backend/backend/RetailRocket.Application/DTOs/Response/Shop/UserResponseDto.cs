namespace RetailRocket.Application.DTOs.Response.Shop;

public record UserResponseDto
{
    public Guid UserId { get; set;  }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public DateTime CreatedAt { get; set; }
}