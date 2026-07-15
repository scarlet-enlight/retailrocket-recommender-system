using RetailRocket.Application.Interfaces.ML;
using RetailRocket.Domain.Entities.ML;

namespace RetailRocket.Application.Services.ML;

public class RecommendationRuleService
{
    private readonly IRecommendationRuleRepository _recommendationRuleRepository;

    public RecommendationRuleService(IRecommendationRuleRepository recommendationRuleRepository) => 
        _recommendationRuleRepository = recommendationRuleRepository;

    public async Task<IEnumerable<RecommendationRule>> GetAllRecommendationRulesAsync() =>
        await _recommendationRuleRepository.GetAllAsync();

    public async Task<RecommendationRule?> GetRecommendationRuleAsync(Guid id) =>
        await _recommendationRuleRepository.GetByIdAsync(id);
    
    public async Task<IEnumerable<RecommendationRule>> GetRecommendationRulesByRequiredItemAsync(int id) =>
        await _recommendationRuleRepository.GetByIfItemIdAsync(id);
    
    public async Task<IEnumerable<RecommendationRule>> GetRecommendationRulesByResultItemAsync(int id) =>
        await _recommendationRuleRepository.GetByThenItemIdAsync(id);
    
    public async Task AddRecommendationRuleAsync(RecommendationRule recommendationRule) => 
        await _recommendationRuleRepository.AddAsync(recommendationRule);
    
    public async Task UpdateRecommendationRuleAsync(RecommendationRule recommendationRule) =>
        await _recommendationRuleRepository.UpdateAsync(recommendationRule);
    
    public async Task DeleteRecommendationRuleAsync(Guid id) =>
        await _recommendationRuleRepository.DeleteAsync(id);
}