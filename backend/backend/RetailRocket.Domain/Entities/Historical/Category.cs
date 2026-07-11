namespace RetailRocket.Domain.Entities.Historical;

public class Category
{
    public int CategoryId { get; }
    public int? ParentCategoryId { get; private set; }
    public Category? ParentCategory { get; private set; }
    public string? Name { get; private set; }

    public Category(int? parentCategoryId, string? name)
    {
        ParentCategoryId = parentCategoryId;
        Name = name;
    }
}