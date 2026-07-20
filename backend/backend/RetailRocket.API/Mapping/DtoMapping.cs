using RetailRocket.Application.DTOs.Short.Historical;
using RetailRocket.Domain.Entities.Historical;

namespace RetailRocket.API.Mapping;

public class DtoMapping
{
    public static CategoryShortDto? MapCategory(Category? category)
    {
        if (category is null) return null;

        return new CategoryShortDto
        {
            Name = category.Name,
            ParentCategory = MapCategory(category.ParentCategory)
        };
    }
}