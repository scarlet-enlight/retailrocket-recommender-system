using RetailRocket.Domain.Entities.Historical;

namespace RetailRocket.Application.DTOs.Response.ML;

public class RecommendationRuleDto
{
    public Item? IfItem { get; private set; }
    public Item? ThenItem { get; private set; }
    public float Support { get; private set; }
    public float Confidence { get; private set; }
    public float Lift { get; private set; }
}