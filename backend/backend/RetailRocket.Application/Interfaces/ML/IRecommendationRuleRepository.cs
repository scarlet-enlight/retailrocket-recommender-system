using RetailRocket.Domain.Entities.ML;

namespace RetailRocket.Application.Interfaces.ML;

public interface IRecommendationRuleRepository : IRepository<RecommendationRule, Guid>
{
    Task<IEnumerable<RecommendationRule>> GetAllAsync();
    Task<IEnumerable<RecommendationRule>> GetByIfItemIdAsync(int itemId);
    Task<IEnumerable<RecommendationRule>> GetByThenItemIdAsync(int itemId);
    Task UpdateAsync(RecommendationRule recommendationRule);
}