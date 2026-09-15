using AutoMapper;
using RetailRocket.Application.DTOs.Short.Historical;
using RetailRocket.Domain.Entities.Historical;

namespace RetailRocket.Application.Mappings.Short;

public class ItemShortMappingProfile : Profile
{
    public ItemShortMappingProfile()
    {
        CreateMap<Item, ItemShortDto>()
            .ForMember(dest => dest.Category,
                opt => opt.MapFrom(src => src.Category));
    }
}