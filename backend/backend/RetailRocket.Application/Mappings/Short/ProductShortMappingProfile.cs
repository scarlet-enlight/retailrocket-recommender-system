using AutoMapper;
using RetailRocket.Application.DTOs.Short.Shop;
using RetailRocket.Domain.Entities.Shop;

namespace RetailRocket.Application.Mappings.Short;

public class ProductShortMappingProfile : Profile
{
    public ProductShortMappingProfile()
    {
        SourceMemberNamingConvention = new LowerUnderscoreNamingConvention();
        DestinationMemberNamingConvention = new PascalCaseNamingConvention();
        
        CreateMap<Product, ProductShortDto>();
    }
}