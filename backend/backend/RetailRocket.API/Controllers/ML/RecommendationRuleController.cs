using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
    private readonly IMapper _mapper;

    public RecommendationRuleController(RecommendationRuleService recommendationRuleService, IMapper mapper)
    {
        _recommendationRuleService = recommendationRuleService;
        _mapper = mapper;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var recommendationRules = await _recommendationRuleService.GetAllRecommendationRulesAsync();
        var result = _mapper.Map<IEnumerable<RecommendationRuleResponseDto>>(recommendationRules);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var recommendationRule = await _recommendationRuleService.GetRecommendationRuleAsync(id);
        if (recommendationRule is null) return NotFound();
        return Ok(_mapper.Map<RecommendationRuleResponseDto>(recommendationRule));
    }

    // WIP: Refactor get by req/resItem to get all recRules
    [Authorize]
    [HttpGet("by-required-item/{reqItemId}")]
    public async Task<IActionResult> GetByRequiredItem(int reqItemId)
    {
        var recommendationRules = await _recommendationRuleService.GetRecommendationRulesByRequiredItemAsync(reqItemId);
        var result = _mapper.Map<IEnumerable<RecommendationRuleResponseDto>>(recommendationRules);
        return Ok(result);
    }
    
    [Authorize]
    [HttpGet("by-result-item/{resItemId}")]
    public async Task<IActionResult> GetByResultItem(int resItemId)
    {
        var recommendationRules = await _recommendationRuleService.GetRecommendationRulesByResultItemAsync(resItemId);
        var result = _mapper.Map<IEnumerable<RecommendationRuleResponseDto>>(recommendationRules);
        return Ok(result);
    }

    [Authorize]
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
    
    [Authorize]
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
    
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var recommendationRule = await _recommendationRuleService.GetRecommendationRuleAsync(id);
        if (recommendationRule is null) return NotFound();
        await _recommendationRuleService.DeleteRecommendationRuleAsync(id);
        return NoContent();
    }
}