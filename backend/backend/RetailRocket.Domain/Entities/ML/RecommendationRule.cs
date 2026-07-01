using RetailRocket.Domain.Entities.Historical;

namespace RetailRocket.Domain.Entities.ML;

public class RecommendationRule
{
    public Guid RecommendationRuleId { get; }
    public Guid IfItemId { get; private set; }
    public Item? IfItem { get; set; }
    public Guid ThenItemId { get; private set;  }
    public Item? ThenItem { get; set; }
    public double Support { get; private set; }
    public double Confidence { get; private set; }
    public double Lift { get; private set; }
    public DateTime CreatedAt { get; }

    public RecommendationRule(Guid ifItemId, Guid thenItemId, double? support, double? confidence, double? lift)
    {
        IfItemId = ifItemId;
        ThenItemId = thenItemId;
        Support = support.GetValueOrDefault();
        Confidence = confidence.GetValueOrDefault();
        Lift = lift.GetValueOrDefault();
    }

    public void UpdateRequiredItem(Guid itemId) =>  IfItemId = itemId;
    public void UpdateTargetItem(Guid itemId) =>  ThenItemId = itemId;
    public void UpdateSupportValue(double? support) =>  Support = support.GetValueOrDefault();
    public void UpdateConfidenceValue(double? confidence) =>  Confidence = confidence.GetValueOrDefault();
    public void UpdateLiftValue(double? lift) =>  Lift = lift.GetValueOrDefault();
}