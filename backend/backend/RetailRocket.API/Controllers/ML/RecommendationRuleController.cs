using Microsoft.AspNetCore.Mvc;
using RetailRocket.Application.DTOs.Request.ML;
using RetailRocket.Application.DTOs.Response.ML;
using RetailRocket.Application.Services.ML;
using RetailRocket.Domain.Entities.ML;

namespace RetailRocket.API.Controllers.ML;

[ApiController]
[Route("api/[controller]")]
public class RecommendationRuleController : ControllerBase
{
    private readonly RecommendationRuleService _recommendationRuleService;
    
    public RecommendationRuleController(RecommendationRuleService recommendationRuleService) =>
        _recommendationRuleService = recommendationRuleService;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var recommendationRules = await _recommendationRuleService.GetAllRecommendationRulesAsync();
        var result = recommendationRules.Select(rr => new RecommendationRuleDto
        {
            IfItemId = rr.IfItemId,
            ThenItemId = rr.ThenItemId,
            Support = rr.Support,
            Confidence = rr.Confidence,
            Lift = rr.Lift
        });
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var recommendationRule = await _recommendationRuleService.GetRecommendationRuleAsync(id);
        if (recommendationRule is null) return NotFound();
        return Ok( new RecommendationRuleDto
        {
            IfItemId = recommendationRule.IfItemId,
            ThenItemId = recommendationRule.ThenItemId,
            Support = recommendationRule.Support,
            Confidence = recommendationRule.Confidence,
            Lift = recommendationRule.Lift
        });
    }

    [HttpGet("by-required-item/{reqItemId}")]
    public async Task<IActionResult> GetByRequiredItem(int reqItemId)
    {
        var recommendationRules = await _recommendationRuleService.GetRecommendationRulesByRequiredItemAsync(reqItemId);
        var result = recommendationRules.Select(rr => new RecommendationRuleDto
        {
            IfItemId = rr.IfItemId,
            ThenItemId = rr.ThenItemId,
            Support = rr.Support,
            Confidence = rr.Confidence,
            Lift = rr.Lift
            
        });
        return Ok(result);
    }
    
    [HttpGet("by-result-item/{resItemId}")]
    public async Task<IActionResult> GetByResultItem(int resItemId)
    {
        var recommendationRules = await _recommendationRuleService.GetRecommendationRulesByResultItemAsync(resItemId);
        var result = recommendationRules.Select(rr => new RecommendationRuleDto
        {
            IfItemId = rr.IfItemId,
            ThenItemId = rr.ThenItemId,
            Support = rr.Support,
            Confidence = rr.Confidence,
            Lift = rr.Lift
            
        });
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRecommendationRuleDto dto)
    {
        var recommendationRule = new RecommendationRule(dto.IfItemId, dto.ThenItemId, dto.Support, dto.Confidence, dto.Lift);
        await _recommendationRuleService.AddRecommendationRuleAsync(recommendationRule);
        return CreatedAtAction(nameof(GetById), new { id = recommendationRule.RecommendationRuleId }, new RecommendationRuleDto
        {
            IfItemId = recommendationRule.IfItemId,
            ThenItemId = recommendationRule.ThenItemId,
            Support = recommendationRule.Support,
            Confidence = recommendationRule.Confidence,
            Lift = recommendationRule.Lift
        });
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateRecommendationRuleDto dto)
    {
        var recommendationRule = await _recommendationRuleService.GetRecommendationRuleAsync(id);
        if (recommendationRule is null) return NotFound();
        recommendationRule.UpdateRequiredItem(dto.IfItemId);
        recommendationRule.UpdateTargetItem(dto.ThenItemId);
        recommendationRule.UpdateSupportValue(dto.Support);
        recommendationRule.UpdateConfidenceValue(dto.Confidence);
        recommendationRule.UpdateLiftValue(dto.Lift);
        await _recommendationRuleService.UpdateRecommendationRuleAsync(recommendationRule);
        return NoContent();
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var recommendationRule = await _recommendationRuleService.GetRecommendationRuleAsync(id);
        if (recommendationRule is null) return NotFound();
        await _recommendationRuleService.DeleteRecommendationRuleAsync(id);
        return NoContent();
    }
}