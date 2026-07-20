namespace RetailRocket.Application.DTOs.Response.Historical;

public record CategoryResponseDto
{
    public int CategoryId { get; set; }
    public string? Name { get; set; }
}