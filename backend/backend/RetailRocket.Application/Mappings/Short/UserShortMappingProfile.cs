using AutoMapper;
using RetailRocket.Application.DTOs.Short.Shop;
using RetailRocket.Domain.Entities.Shop;

namespace RetailRocket.Application.Mappings.Short;

public class UserShortMappingProfile : Profile
{
    public UserShortMappingProfile()
    {
        SourceMemberNamingConvention = new LowerUnderscoreNamingConvention();
        DestinationMemberNamingConvention = new PascalCaseNamingConvention();
        
        CreateMap<User, UserShortDto>();
    }
}