using AutoMapper;
using RetailRocket.Application.DTOs.Short.Historical;
using RetailRocket.Domain.Entities.Historical;

namespace RetailRocket.Application.Mappings.Short;

public class ItemShortMappingProfile : Profile
{
    public ItemShortMappingProfile()
    {
        SourceMemberNamingConvention = new LowerUnderscoreNamingConvention();
        DestinationMemberNamingConvention = new PascalCaseNamingConvention();
        
        CreateMap<Item, ItemShortDto>()
            .ForMember(dest => dest.ItemId,
            opt => opt.MapFrom(src => src.item_id))
            .ForMember(dest => dest.Category,
                opt => opt.MapFrom(src => src.category))
            .ForMember(dest => dest.IsAvailable,
                opt => opt.MapFrom(src => src.available));
    }
}