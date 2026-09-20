using AutoMapper;
using RetailRocket.Application.DTOs.Response.ML;
using RetailRocket.Domain.Entities.ML;

namespace RetailRocket.Application.Mappings.ML;

public class RecommendationRuleMappingProfile : Profile
{
    public RecommendationRuleMappingProfile()
    {
        CreateMap<RecommendationRule, RecommendationRuleResponseDto>()
            .ForMember(dest => dest.IfItem,
                opt => opt.MapFrom(src => src.IfItem))
            .ForMember(dest => dest.ThenItem,
                opt => opt.MapFrom(src => src.ThenItem));
    }
}