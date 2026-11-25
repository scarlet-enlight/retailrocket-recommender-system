using Microsoft.EntityFrameworkCore;
using RetailRocket.Application.Interfaces.Shop;
using RetailRocket.Domain.Entities.Shop;
using RetailRocket.Infrastructure.Persistence;

namespace RetailRocket.Infrastructure.Repositories.Shop;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;
    
    public UserRepository(AppDbContext context)
    {
        _dbContext = context;
    }
    
    public async Task<IEnumerable<User>> GetAllAsync() => 
        await _dbContext.Users
            .ToListAsync();
    
    public async Task<User?> GetByIdAsync(Guid id) => 
        await _dbContext.Users
            .FirstOrDefaultAsync(u => u.user_id == id);
    
    public async Task<User?> GetByUsernameAsync(string username) =>
        await _dbContext.Users
            .FirstOrDefaultAsync(u => u.username == username);
    
    public async Task<User?> GetByEmailAsync(string email) =>
        await _dbContext.Users
            .FirstOrDefaultAsync(u => u.email == email);


    public async Task AddAsync(User user)
    {
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var user = await GetByIdAsync(id);
        if (user is null) return;
        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();
    }
}   