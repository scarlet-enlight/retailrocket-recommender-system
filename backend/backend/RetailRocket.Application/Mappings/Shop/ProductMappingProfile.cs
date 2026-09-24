using AutoMapper;
using RetailRocket.Application.DTOs.Response.Shop;
using RetailRocket.Application.Mappings.Short;
using RetailRocket.Domain.Entities.Shop;

namespace RetailRocket.Application.Mappings.Shop;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, ProductResponseDto>()
            .ForMember(dest => dest.IsAvailable,
                opt => opt.MapFrom(src =>  src.Item != null && src.Item.IsAvailable))
            .ForMember(dest => dest.Category,
                opt => opt.MapFrom(src => src.Item != null ? src.Category.Name : null));

        var itemShort = new ItemShortMappingProfile();
        var categoryShort = new CategoryShortMappingProfile();
    }
}