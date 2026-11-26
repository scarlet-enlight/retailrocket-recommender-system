using AutoMapper;
using RetailRocket.Application.DTOs.Response.Shop.Products;
using RetailRocket.Application.Mappings.Short;
using RetailRocket.Domain.Entities.Shop;

namespace RetailRocket.Application.Mappings.Shop;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        SourceMemberNamingConvention = new LowerUnderscoreNamingConvention();
        DestinationMemberNamingConvention = new PascalCaseNamingConvention();
        
        CreateMap<Product, ProductResponseDto>()
            .ForMember(dest => dest.ProductId,
                opt => opt.MapFrom(src => src.product_id))
            .ForMember(dest => dest.Category,
                opt => opt.MapFrom(src => src.item != null ? src.item.category : null))
            .ForMember(dest => dest.IsAvailable,
                opt => opt.MapFrom(src => src.item != null && src.item.available));
            

        var itemShort = new ItemShortMappingProfile();
        var categoryShort = new CategoryShortMappingProfile();
    }
}