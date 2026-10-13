using AutoMapper;
using RetailRocket.Application.DTOs.Response.Shop.Products;
using RetailRocket.Application.Mappings.Short;
using RetailRocket.Domain.Entities.Shop;

namespace RetailRocket.Application.Mappings.Shop;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, ProductResponseDto>()
            .ForMember(dest => dest.Category,
                opt => opt.MapFrom(src => src.Item != null ? src.Item.Category : null))
            .ForMember(dest => dest.IsAvailable,
                opt => opt.MapFrom(src => src.Item != null && src.Item.IsAvailable));
            

        var itemShort = new ItemShortMappingProfile();
        var categoryShort = new CategoryShortMappingProfile();
    }
}