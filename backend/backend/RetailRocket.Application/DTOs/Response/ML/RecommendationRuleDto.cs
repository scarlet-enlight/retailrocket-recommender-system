namespace RetailRocket.Application.DTOs.Response.ML;

public class RecommendationRuleDto
{
    public Guid IfItemId { get; set; }
    public Guid ThenItemId { get; set; }
    public double Support { get; set; }
    public double Confidence { get; set; }
    public double Lift { get; set; }
}