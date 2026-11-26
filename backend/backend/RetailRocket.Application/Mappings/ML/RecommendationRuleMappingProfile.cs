using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using RetailRocket.Application.DTOs.Response.ML;
using RetailRocket.Domain.Entities.ML;

namespace RetailRocket.Application.Mappings.ML;

public class RecommendationRuleMappingProfile : Profile
{
    public RecommendationRuleMappingProfile()
    {
        SourceMemberNamingConvention = new LowerUnderscoreNamingConvention();
        DestinationMemberNamingConvention = new PascalCaseNamingConvention();
        
        CreateMap<RecommendationRule, RecommendationRuleResponseDto>()
            .ForMember(dest => dest.RecommendationRuleId,
                opt => opt.MapFrom(src => src.rule_id))
            .ForMember(dest => dest.IfItem,
                opt => opt.MapFrom(src => src.if_item))
            .ForMember(dest => dest.ThenItem,
                opt => opt.MapFrom(src => src.then_item));
    }
}