using RetailRocket.Application.Interfaces.Shop;
using RetailRocket.Domain.Entities.Shop;

namespace RetailRocket.Application.Services.Shop;

public class UserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync() =>
        await _userRepository.GetAllAsync();

    public async Task<User?> GetUserAsync(int id) =>
        await _userRepository.GetByIdAsync(id);
    
    public async Task<User?> GetUserByUsernameAsync(string username) =>
        await _userRepository.GetByUsernameAsync(username);
    
    public async Task<User?> GetUserByEmailAsync(string email) =>
        await _userRepository.GetByEmailAsync(email);
    
    public async Task AddUserAsync(User user) => 
        await _userRepository.AddAsync(user);
    
    public async Task UpdateUserAsync(User user) =>
        await _userRepository.UpdateAsync(user);
    
    public async Task DeleteUserAsync(int id) =>
        await _userRepository.DeleteAsync(id);
}