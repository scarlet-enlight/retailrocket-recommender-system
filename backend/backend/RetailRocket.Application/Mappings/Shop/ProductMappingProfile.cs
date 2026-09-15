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
            .ForMember(dest => dest.Item, 
                opt => opt.MapFrom(src => src.Item));

        var itemShort = new ItemShortMappingProfile();
        var categoryShort = new CategoryShortMappingProfile();
    }
}