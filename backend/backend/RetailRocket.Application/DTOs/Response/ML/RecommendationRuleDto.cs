using RetailRocket.Domain.Entities.Historical;

namespace RetailRocket.Application.DTOs.Response.ML;

public class RecommendationRuleDto
{
    public Item? IfItem { get; set; }
    public Item? ThenItem { get; set; }
    public float Support { get; set; }
    public float Confidence { get; set; }
    public float Lift { get; set; }
}