using RetailRocket.Domain.Entities.Historical;

namespace RetailRocket.Domain.Entities.ML;

public class RecommendationRule
{
    public Guid RecommendationRuleId { get; }
    public Guid IfItemId { get; private set; }
    public Item? IfItem { get; private set; }
    public Guid ThenItemId { get; private set; }
    public Item? ThenItem { get; private set; }
    public float Support { get; private set; }
    public float Confidence { get; private set; }
    public float Lift { get; private set; }
    public DateTime CreatedAt { get; }
}