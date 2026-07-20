using RetailRocket.Application.DTOs.Short.Historical;

namespace RetailRocket.Application.DTOs.Response.ML;

public record RecommendationRuleResponseDto
{
    public Guid RecommendationRuleId { get; set; }
    public ItemShortDto IfItem { get; set; }
    public ItemShortDto ThenItem { get; set; }
    public double Support { get; set; }
    public double Confidence { get; set; }
    public double Lift { get; set; }
}