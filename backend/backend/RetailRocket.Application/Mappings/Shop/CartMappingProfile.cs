using AutoMapper;
using RetailRocket.Application.DTOs.Response.Shop;
using RetailRocket.Application.Mappings.Short;
using RetailRocket.Domain.Entities.Shop;

namespace RetailRocket.Application.Mappings.Shop;

public class CartMappingProfile : Profile
{
    public CartMappingProfile()
    {
        CreateMap<Cart, CartResponseDto>()
            .ForMember(dest => dest.User,
                opt => opt.MapFrom(src => src.User))
            .ForMember(dest => dest.Product,
                opt => opt.MapFrom(src => src.Product));
    }
}