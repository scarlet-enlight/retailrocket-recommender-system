namespace RetailRocket.Domain.Entities.Historical;

public class Category
{
    public Guid CategoryId { get; }
    public Guid ParentCategoryId { get; private set; }
    public Category? ParentCategory { get; private set; }
    public string? Name { get; private set; }
}