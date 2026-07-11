namespace RetailRocket.Application.DTOs.Request.ML;

public class CreateRecommendationRuleDto
{
    public int IfItemId { get; set; }
    public int ThenItemId { get; set; }
    public double Support { get; set; }
    public double Confidence { get; set; }
    public double Lift { get; set; }
}