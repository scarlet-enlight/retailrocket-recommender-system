namespace RetailRocket.Application.DTOs.Request.ML;

public class CreateRecommendationRuleDto
{
    public Guid IfItemId { get; set; }
    public Guid ThenItemId { get; set; }
    public double Support { get; set; }
    public double Confidence { get; set; }
    public double Lift { get; set; }
}