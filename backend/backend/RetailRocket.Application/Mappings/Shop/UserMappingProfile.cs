using AutoMapper;
using RetailRocket.Application.DTOs.Response.Shop;
using RetailRocket.Domain.Entities.Shop;

namespace RetailRocket.Application.Mappings.Shop;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserResponseDto>();
    }
}