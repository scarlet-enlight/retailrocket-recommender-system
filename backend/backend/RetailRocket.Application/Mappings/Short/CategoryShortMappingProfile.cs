using AutoMapper;
using RetailRocket.Application.DTOs.Short.Historical;
using RetailRocket.Domain.Entities.Historical;

namespace RetailRocket.Application.Mappings.Short;

public class CategoryShortMappingProfile : Profile
{
    public CategoryShortMappingProfile()
    {
        CreateMap<Category, CategoryShortDto>()
            .ForMember(dest => dest.ParentCategory,
                opt => opt.MapFrom(src => src.ParentCategory));
    }
}