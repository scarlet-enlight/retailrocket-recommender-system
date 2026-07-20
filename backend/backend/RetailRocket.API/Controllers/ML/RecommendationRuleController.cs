using Microsoft.AspNetCore.Mvc;
using RetailRocket.API.Mapping;
using RetailRocket.Application.DTOs.Request.ML;
using RetailRocket.Application.DTOs.Response.ML;
using RetailRocket.Application.DTOs.Short.Historical;
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
        var result = recommendationRules.Select(rr => new RecommendationRuleResponseDto
        {
            IfItem = new ItemShortDto
            {
                ItemId = rr.IfItemId,
                Category = DtoMapping.MapCategory(rr.IfItem.Category),
                IsAvailable = rr.IfItem.IsAvailable
            },
            ThenItem = new ItemShortDto
            {
                ItemId = rr.ThenItemId,
                Category = DtoMapping.MapCategory(rr.ThenItem.Category),
                IsAvailable = rr.ThenItem.IsAvailable
            },
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
        return Ok( new RecommendationRuleResponseDto
        {
            IfItem = new ItemShortDto
            {
                ItemId = recommendationRule.IfItemId,
                Category = DtoMapping.MapCategory(recommendationRule.IfItem.Category),
                IsAvailable = recommendationRule.IfItem.IsAvailable
            },
            ThenItem = new ItemShortDto
            {
                ItemId = recommendationRule.ThenItemId,
                Category = DtoMapping.MapCategory(recommendationRule.ThenItem.Category),
                IsAvailable = recommendationRule.ThenItem.IsAvailable
            },
            Support = recommendationRule.Support,
            Confidence = recommendationRule.Confidence,
            Lift = recommendationRule.Lift
        });
    }

    // WIP: Refactor get by req/resItem to get all recRules
    [HttpGet("by-required-item/{reqItemId}")]
    public async Task<IActionResult> GetByRequiredItem(int reqItemId)
    {
        var recommendationRules = await _recommendationRuleService.GetRecommendationRulesByRequiredItemAsync(reqItemId);
        var result = recommendationRules.Select(rr => new RecommendationRuleResponseDto
        {
            IfItem = new ItemShortDto
            {
                ItemId = rr.IfItemId,
                Category = DtoMapping.MapCategory(rr.IfItem.Category),
                IsAvailable = rr.IfItem.IsAvailable
            },
            ThenItem = new ItemShortDto
            {
                ItemId = rr.ThenItemId,
                Category = DtoMapping.MapCategory(rr.ThenItem.Category),
                IsAvailable = rr.ThenItem.IsAvailable
            },
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
        var result = recommendationRules.Select(rr => new RecommendationRuleResponseDto
        {
            IfItem = new ItemShortDto
            {
                ItemId = rr.IfItemId,
                Category = DtoMapping.MapCategory(rr.ThenItem.Category),
                IsAvailable = rr.IfItem.IsAvailable
            },
            ThenItem = new ItemShortDto
            {
                ItemId = rr.ThenItemId,
                Category = DtoMapping.MapCategory(rr.ThenItem.Category),
                IsAvailable = rr.ThenItem.IsAvailable
            },
            Support = rr.Support,
            Confidence = rr.Confidence,
            Lift = rr.Lift
            
        });
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RecommendationRuleRequestDto requestDto)
    {
        var recommendationRule = new RecommendationRule(requestDto.IfItemId, requestDto.ThenItemId, requestDto.Support, requestDto.Confidence, requestDto.Lift);
        await _recommendationRuleService.AddRecommendationRuleAsync(recommendationRule);
        
        return CreatedAtAction(nameof(GetById), new { id = recommendationRule.RecommendationRuleId }, new RecommendationRuleResponseDto
        {
            IfItem = new ItemShortDto
            {
                ItemId = recommendationRule.IfItemId,
                Category = DtoMapping.MapCategory(recommendationRule.IfItem.Category),
                IsAvailable = recommendationRule.IfItem.IsAvailable
            },
            ThenItem = new ItemShortDto
            {
                ItemId = recommendationRule.ThenItemId,
                Category = DtoMapping.MapCategory(recommendationRule.ThenItem.Category),
                IsAvailable = recommendationRule.ThenItem.IsAvailable
            },
            Support = recommendationRule.Support,
            Confidence = recommendationRule.Confidence,
            Lift = recommendationRule.Lift
        });
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] RecommendationRuleRequestDto requestDto)
    {
        var recommendationRule = await _recommendationRuleService.GetRecommendationRuleAsync(id);
        if (recommendationRule is null) return NotFound();
        recommendationRule.UpdateRequiredItem(requestDto.IfItemId);
        recommendationRule.UpdateTargetItem(requestDto.ThenItemId);
        recommendationRule.UpdateSupportValue(requestDto.Support);
        recommendationRule.UpdateConfidenceValue(requestDto.Confidence);
        recommendationRule.UpdateLiftValue(requestDto.Lift);
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