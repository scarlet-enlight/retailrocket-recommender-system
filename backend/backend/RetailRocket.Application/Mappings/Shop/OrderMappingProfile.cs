using AutoMapper;
using RetailRocket.Application.DTOs.Response.Shop;
using RetailRocket.Domain.Entities.Shop;

namespace RetailRocket.Application.Mappings.Shop;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        CreateMap<Order, OrderResponseDto>();
    }
}