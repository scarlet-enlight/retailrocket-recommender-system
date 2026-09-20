using RetailRocket.Domain.Entities.Historical;

namespace RetailRocket.Domain.Entities.ML;

public class RecommendationRule
{
    public Guid RecommendationRuleId { get; }
    public int IfItemId { get; private set; }
    public Item IfItem { get; set; }
    public int ThenItemId { get; private set;  }
    public Item ThenItem { get; set; }
    public double Support { get; private set; }
    public double Confidence { get; private set; }
    public double Lift { get; private set; }
    public DateTime CreatedAt { get; }

    public RecommendationRule(int ifItemId, int thenItemId, double? support, double? confidence, double? lift)
    {
        IfItemId = ifItemId;
        ThenItemId = thenItemId;
        Support = support.GetValueOrDefault();
        Confidence = confidence.GetValueOrDefault();
        Lift = lift.GetValueOrDefault();
    }

    public void UpdateRequiredItem(int itemId) =>  IfItemId = itemId;
    public void UpdateTargetItem(int itemId) =>  ThenItemId = itemId;
    public void UpdateSupportValue(double? support) =>  Support = support.GetValueOrDefault();
    public void UpdateConfidenceValue(double? confidence) =>  Confidence = confidence.GetValueOrDefault();
    public void UpdateLiftValue(double? lift) =>  Lift = lift.GetValueOrDefault();
}