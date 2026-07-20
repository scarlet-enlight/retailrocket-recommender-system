namespace RetailRocket.Application.DTOs.Short.Historical;

public record CategoryShortDto()
{
    public CategoryShortDto? ParentCategory { get; set; }
    public string? Name { get; set; }
}