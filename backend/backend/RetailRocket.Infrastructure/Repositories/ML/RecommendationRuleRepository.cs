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
            .Include(rr => rr.IfItem)
            .Include(rr => rr.ThenItem)
            .Include(rr => rr.Support)
            .Include(rr => rr.Lift)
            .Include(rr => rr.Confidence)
            .Include(rr => rr.CreatedAt)
            .FirstOrDefaultAsync(rr => rr.RecommendationRuleId == id);

    public async Task<IEnumerable<RecommendationRule>> GetAllAsync() =>
        await _dbContext.RecommendationRules
            .Include(rr => rr.IfItem)
            .Include(rr => rr.ThenItem)
            .Include(rr => rr.Support)
            .Include(rr => rr.Lift)
            .Include(rr => rr.Confidence)
            .ToListAsync();
    
    public async Task<IEnumerable<RecommendationRule>> GetByIfItemIdAsync(Guid ifItemId) =>
        await _dbContext.RecommendationRules
            .Include(rr => rr.IfItem)
            .Include(rr => rr.ThenItem)
            .Include(rr => rr.Support)
            .Include(rr => rr.Lift)
            .Include(rr => rr.Confidence)
            .Where(rr => rr.IfItemId == ifItemId)
            .ToListAsync();
    
    public async Task<IEnumerable<RecommendationRule>> GetByThenItemIdAsync(Guid thenItemId) =>
        await _dbContext.RecommendationRules
            .Include(rr => rr.IfItem)
            .Include(rr => rr.ThenItem)
            .Include(rr => rr.Support)
            .Include(rr => rr.Lift)
            .Include(rr => rr.Confidence)
            .Where(rr => rr.ThenItemId == thenItemId)
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