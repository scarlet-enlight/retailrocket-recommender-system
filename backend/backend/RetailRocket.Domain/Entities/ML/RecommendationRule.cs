using RetailRocket.Domain.Entities.Historical;

namespace RetailRocket.Domain.Entities.ML;

public class RecommendationRule
{
    public Guid RecommendationRuleId { get; }
    public Guid IfItemId { get; }
    public Item? IfItem { get; private set; }
    public Guid ThenItemId { get; }
    public Item? ThenItem { get; private set; }
    public float Support { get; private set; }
    public float Confidence { get; private set; }
    public float Lift { get; private set; }
    public DateTime CreatedAt { get; }

    public RecommendationRule(Item? ifItem, Item? thenItem, float? support, float? confidence, float? lift)
    {
        IfItem = ifItem;
        ThenItem = thenItem;
        Support = support.GetValueOrDefault();
        Confidence = confidence.GetValueOrDefault();
        Lift = lift.GetValueOrDefault();
    }

    public void UpdateRequiredItem(Item? item) =>  IfItem = item;
    public void UpdateTargetItem(Item? item) =>  ThenItem = item;
    public void UpdateSupportValue(float? support) =>  Support = support.GetValueOrDefault();
    public void UpdateConfidenceValue(float? confidence) =>  Confidence = confidence.GetValueOrDefault();
    public void UpdateLiftValue(float? lift) =>  Lift = lift.GetValueOrDefault();
}