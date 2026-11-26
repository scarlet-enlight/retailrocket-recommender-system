using Microsoft.EntityFrameworkCore;
using RetailRocket.Application.Interfaces.ML;
using RetailRocket.Domain.Entities.ML;
using RetailRocket.Infrastructure.Persistence;

namespace RetailRocket.Infrastructure.Repositories.ML;

public class RecommendationRuleRepository : IRecommendationRuleRepository
{
    private readonly AppDbContext _dbContext;
    
    public RecommendationRuleRepository(AppDbContext context)
    {
        _dbContext = context;
    }

    public async Task<RecommendationRule?> GetByIdAsync(Guid id) =>
        await _dbContext.RecommendationRules
            .Include(rr => rr.if_item).ThenInclude(i => i.category).ThenInclude(c => c.parent_category)
            .Include(rr => rr.then_item).ThenInclude(i => i.category).ThenInclude(c => c.parent_category)
            .FirstOrDefaultAsync(rr => rr.rule_id == id);

    public async Task<IEnumerable<RecommendationRule>> GetAllAsync() =>
        await _dbContext.RecommendationRules
            .Include(rr => rr.if_item).ThenInclude(i => i.category).ThenInclude(c => c.parent_category)
            .Include(rr => rr.then_item).ThenInclude(i => i.category).ThenInclude(c => c.parent_category)
            .ToListAsync();
    
    public async Task<IEnumerable<RecommendationRule>> GetByIfItemIdAsync(int ifItemId) =>
        await _dbContext.RecommendationRules
            .Include(rr => rr.if_item).ThenInclude(i => i.category).ThenInclude(c => c.parent_category)
            .Include(rr => rr.then_item).ThenInclude(i => i.category).ThenInclude(c => c.parent_category)
            .Where(rr => rr.if_item_id == ifItemId)
            .ToListAsync();
    
    public async Task<IEnumerable<RecommendationRule>> GetByThenItemIdAsync(int thenItemId) =>
        await _dbContext.RecommendationRules
            .Include(rr => rr.if_item).ThenInclude(i => i.category).ThenInclude(c => c.parent_category)
            .Include(rr => rr.then_item).ThenInclude(i => i.category).ThenInclude(c => c.parent_category)
            .Where(rr => rr.then_item_id == thenItemId)
            .ToListAsync();

    public async Task AddAsync(RecommendationRule recommendationRule)
    {
        await _dbContext.RecommendationRules.AddAsync(recommendationRule);
        await _dbContext.SaveChangesAsync();
    }

    public Task UpdateAsync(RecommendationRule recommendationRule)
    {
        _dbContext.RecommendationRules.Update(recommendationRule);
        return _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var recommendationRule = await GetByIdAsync(id);
        if (recommendationRule is null) return;
        _dbContext.RecommendationRules.Remove(recommendationRule);
        await _dbContext.SaveChangesAsync();
    }
}