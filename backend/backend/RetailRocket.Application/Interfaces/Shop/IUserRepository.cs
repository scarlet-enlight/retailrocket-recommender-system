using RetailRocket.Domain.Entities.Shop;

namespace RetailRocket.Application.Interfaces.Shop;

public interface IUserRepository : IRepository<User,  Guid>
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task UpdateAsync(User user);
}