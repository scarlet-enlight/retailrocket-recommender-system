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
            IfItem = rr.IfItem,
            ThenItem = rr.ThenItem,
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
            IfItem = recommendationRule.IfItem,
            ThenItem = recommendationRule.ThenItem,
            Support = recommendationRule.Support,
            Confidence = recommendationRule.Confidence,
            Lift = recommendationRule.Lift
        });
    }

    [HttpGet("{by-required-item}")]
    public async Task<IActionResult> GetByRequiredItem(Guid reqItemId)
    {
        var recommendationRules = await _recommendationRuleService.GetRecommendationRulesByRequiredItemAsync(reqItemId);
        var result = recommendationRules.Select(rr => new RecommendationRuleDto
        {
            IfItem = rr.IfItem,
            ThenItem = rr.ThenItem,
            Support = rr.Support,
            Confidence = rr.Confidence,
            Lift = rr.Lift
            
        });
        return Ok(result);
    }
    
    [HttpGet("{by-required-item}")]
    public async Task<IActionResult> GetByResultItem(Guid resItemId)
    {
        var recommendationRules = await _recommendationRuleService.GetRecommendationRulesByResultItemAsync(resItemId);
        var result = recommendationRules.Select(rr => new RecommendationRuleDto
        {
            IfItem = rr.IfItem,
            ThenItem = rr.ThenItem,
            Support = rr.Support,
            Confidence = rr.Confidence,
            Lift = rr.Lift
            
        });
        return Ok(result);
    }

    [HttpPost("{id}")]
    public async Task<IActionResult> Create([FromBody] CreateRecommendationRuleDto dto)
    {
        var recommendationRule = new RecommendationRule(dto.IfItem, dto.ThenItem, dto.Support, dto.Confidence, dto.Lift);
        await _recommendationRuleService.AddRecommendationRuleAsync(recommendationRule);
        return CreatedAtAction(nameof(GetById), new { id = recommendationRule.RecommendationRuleId }, new RecommendationRuleDto
        {
            IfItem = recommendationRule.IfItem,
            ThenItem = recommendationRule.ThenItem,
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
        recommendationRule.UpdateRequiredItem(dto.IfItem);
        recommendationRule.UpdateTargetItem(dto.ThenItem);
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