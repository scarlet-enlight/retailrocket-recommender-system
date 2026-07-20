namespace RetailRocket.Application.DTOs.Short.Historical;

public record ItemShortDto()
{
    public int ItemId { get; set; }
    public CategoryShortDto? Category { get; set; }
    public bool IsAvailable { get; set; }
}