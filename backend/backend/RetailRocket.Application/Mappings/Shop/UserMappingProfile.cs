using AutoMapper;
using RetailRocket.Application.DTOs.Response.Shop.Users;
using RetailRocket.Domain.Entities.Shop;

namespace RetailRocket.Application.Mappings.Shop;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        SourceMemberNamingConvention = new LowerUnderscoreNamingConvention();
        DestinationMemberNamingConvention = new PascalCaseNamingConvention();
        
        CreateMap<User, UserResponseDto>();
    }
}