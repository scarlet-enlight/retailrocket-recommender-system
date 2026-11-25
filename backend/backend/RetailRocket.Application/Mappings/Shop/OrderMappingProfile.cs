using AutoMapper;
using RetailRocket.Application.DTOs.Response.Shop.Orders;
using RetailRocket.Domain.Entities.Shop;

namespace RetailRocket.Application.Mappings.Shop;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        SourceMemberNamingConvention = new LowerUnderscoreNamingConvention();
        DestinationMemberNamingConvention = new PascalCaseNamingConvention();
        
        CreateMap<Order, OrderResponseDto>();
    }
}