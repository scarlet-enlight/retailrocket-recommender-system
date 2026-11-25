namespace RetailRocket.Domain.Entities.Historical;

public class Category
{
    public int category_id { get; }
    public int? parent_id { get; private set; }
    public Category? parent_category { get; private set; }
    public string? name { get; private set; }

    public Category(int? parent_id, string? name)
    {
        this.parent_id = parent_id;
        this.name = name;
    }
}