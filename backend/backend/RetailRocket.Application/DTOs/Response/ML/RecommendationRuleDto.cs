namespace RetailRocket.Application.DTOs.Response.ML;

public class RecommendationRuleDto
{
    public int IfItemId { get; set; }
    public int ThenItemId { get; set; }
    public double Support { get; set; }
    public double Confidence { get; set; }
    public double Lift { get; set; }
}