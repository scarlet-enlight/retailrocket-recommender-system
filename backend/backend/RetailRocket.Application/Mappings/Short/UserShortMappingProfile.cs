using AutoMapper;
using RetailRocket.Application.DTOs.Short.Shop;
using RetailRocket.Domain.Entities.Shop;

namespace RetailRocket.Application.Mappings.Short;

public class UserShortMappingProfile : Profile
{
    public UserShortMappingProfile()
    {
        CreateMap<User, UserShortDto>();
    }
}