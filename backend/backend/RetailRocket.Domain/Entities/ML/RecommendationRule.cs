using RetailRocket.Domain.Entities.Historical;

namespace RetailRocket.Domain.Entities.ML;

public class RecommendationRule
{
    public Guid rule_id { get; }
    public int if_item_id { get; private set; }
    public Item? if_item { get; set; }
    public int then_item_id { get; private set;  }
    public Item? then_item { get; set; }
    public double support { get; private set; }
    public double confidence { get; private set; }
    public double lift { get; private set; }
    public DateTime created_at { get; }

    public RecommendationRule(int if_item_id, int then_item_id, double support, double confidence, double lift)
    {
        this.if_item_id = if_item_id;
        this.then_item_id = then_item_id;
        this.support = support;
        this.confidence = confidence;
        this.lift = lift;
    }

    public void UpdateRequiredItem(int if_item_id) =>  this.if_item_id = if_item_id;
    public void UpdateTargetItem(int then_item_id) =>  this.then_item_id = then_item_id;
    public void UpdateSupportValue(double? support) =>  this.support = support.GetValueOrDefault();
    public void UpdateConfidenceValue(double? confidence) =>  this.confidence = confidence.GetValueOrDefault();
    public void UpdateLiftValue(double? lift) =>  this.lift = lift.GetValueOrDefault();
}