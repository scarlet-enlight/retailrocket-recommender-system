namespace RetailRocket.Application.DTOs.Response.Historical;

public class ItemDto
{
    public int ItemId { get; set; }
    public bool IsAvailable { get; set; }
    public string? CategoryName { get; set; }
}