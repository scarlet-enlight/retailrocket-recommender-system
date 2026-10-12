using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailRocket.Application.DTOs.Request.ML;
using RetailRocket.Application.DTOs.Response.ML;
using RetailRocket.Application.Services.ML;
using RetailRocket.Domain.Entities.ML;
using RetailRocket.Domain.Exceptions;

namespace RetailRocket.API.Controllers.ML;

[ApiController]
[Route("api/[controller]")]
public class RecommendationRuleController : ControllerBase
{
    private readonly RecommendationRuleService _recommendationRuleService;
    private readonly IMapper _mapper;

    private const double Epsilon = 1e-10;
    private static bool IsProvided(double value) => Math.Abs(value) > Epsilon;
    private static bool HasChanged(double newVal, double oldVal) => Math.Abs(newVal - oldVal) > Epsilon;

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
        if (id == Guid.Empty)
            throw new ValidationException("Recommendation Rule ID is required.");
        
        var recommendationRule = await _recommendationRuleService.GetRecommendationRuleAsync(id);
        if (recommendationRule is null) return NotFound();
        return Ok(_mapper.Map<RecommendationRuleResponseDto>(recommendationRule));
    }

    // WIP: Refactor get by req/resItem to get all recRules
    [Authorize]
    [HttpGet("by-required-item/{reqItemId}")]
    public async Task<IActionResult> GetByRequiredItem(int reqItemId)
    {
        if (reqItemId == 0)
            throw new ValidationException("Required Item ID is required.");
        
        var recommendationRules = await _recommendationRuleService.GetRecommendationRulesByRequiredItemAsync(reqItemId);
        var result = _mapper.Map<IEnumerable<RecommendationRuleResponseDto>>(recommendationRules);
        return Ok(result);
    }
    
    [Authorize]
    [HttpGet("by-result-item/{resItemId}")]
    public async Task<IActionResult> GetByResultItem(int resItemId)
    {
        if (resItemId == 0)
            throw new ValidationException("Required Item ID is required.");
        
        var recommendationRules = await _recommendationRuleService.GetRecommendationRulesByResultItemAsync(resItemId);
        var result = _mapper.Map<IEnumerable<RecommendationRuleResponseDto>>(recommendationRules);
        return Ok(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RecommendationRuleCreateRequestDto createRequestDto)
    {
        if (createRequestDto.IfItemId <= 0 || createRequestDto.ThenItemId <= 0)
            throw new ValidationException("Recommendation Rule Items are required.");

        if (!IsProvided(createRequestDto.Support) || !IsProvided(createRequestDto.Confidence) || !IsProvided(createRequestDto.Lift))
            throw new ValidationException("Support, Confidence and Lift values are required.");
        
        if (createRequestDto.Support < 0 || createRequestDto.Support > 1) 
            throw new ValidationException("Recommendation Rule's Support value must be between 0 and 1.");
        
        if (createRequestDto.Confidence < 0 || createRequestDto.Confidence > 1)
            throw new ValidationException("Recommendation Rule's Confidence value must be between 0 and 1.");
        
        if (createRequestDto.Lift < 0)
            throw new ValidationException("Recommendation Rule's Lift value must be greater than 0.");
        
        var recommendationRule = new RecommendationRule(createRequestDto.IfItemId, createRequestDto.ThenItemId, createRequestDto.Support, createRequestDto.Confidence, createRequestDto.Lift);
        await _recommendationRuleService.AddRecommendationRuleAsync(recommendationRule);

        var created =
            await _recommendationRuleService.GetRecommendationRuleAsync(recommendationRule.RecommendationRuleId);
        return CreatedAtAction(nameof(GetById), new { id = created!.RecommendationRuleId },
            _mapper.Map<RecommendationRuleResponseDto>(created));
    }
    
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] RecommendationRuleRequestDto updateRequestDto)
    {
        if (id == Guid.Empty)
            throw new ValidationException("Recommendation Rule ID is required.");
        
        var recommendationRule = await _recommendationRuleService.GetRecommendationRuleAsync(id);
        if (recommendationRule is null)
            throw new NotFoundException("Recommendation Rule not found.");
        
        if (updateRequestDto.IfItemId > 0 && updateRequestDto.IfItemId != recommendationRule.IfItemId)
            recommendationRule.UpdateRequiredItem(updateRequestDto.IfItemId);

        if (updateRequestDto.ThenItemId > 0 && updateRequestDto.ThenItemId != recommendationRule.ThenItemId)
            recommendationRule.UpdateTargetItem(updateRequestDto.ThenItemId);

        if (IsProvided(updateRequestDto.Support))
        {
            if (updateRequestDto.Support < 0 || updateRequestDto.Support > 1)
                throw new ValidationException("Support must be between 0 and 1.");
            if (HasChanged(updateRequestDto.Support, recommendationRule.Support))
                recommendationRule.UpdateSupportValue(updateRequestDto.Support);
        }

        if (IsProvided(updateRequestDto.Confidence))
        {
            if (updateRequestDto.Confidence < 0 || updateRequestDto.Confidence > 1)
                throw new ValidationException("Confidence must be between 0 and 1.");
            if (HasChanged(updateRequestDto.Confidence, recommendationRule.Confidence))
                recommendationRule.UpdateConfidenceValue(updateRequestDto.Confidence);
        }

        if (IsProvided(updateRequestDto.Lift))
        {
            if (updateRequestDto.Lift < 0)
                throw new ValidationException("Lift must be greater than 0.");
            if (HasChanged(updateRequestDto.Lift, recommendationRule.Lift))
                recommendationRule.UpdateLiftValue(updateRequestDto.Lift);
        }
        
        await _recommendationRuleService.UpdateRecommendationRuleAsync(recommendationRule);
        return NoContent();
    }
    
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (id == Guid.Empty)
            throw new ValidationException("Recommendation Rule ID is required.");
        
        var recommendationRule = await _recommendationRuleService.GetRecommendationRuleAsync(id);
        if (recommendationRule is null) 
            throw new NotFoundException("Recommendation Rule not found.");
        await _recommendationRuleService.DeleteRecommendationRuleAsync(id);
        return NoContent();
    }
}