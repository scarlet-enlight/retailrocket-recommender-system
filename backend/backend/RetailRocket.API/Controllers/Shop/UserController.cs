using Microsoft.AspNetCore.Mvc;
using RetailRocket.Application.DTOs.Request.Shop;
using RetailRocket.Application.DTOs.Response.Shop;
using RetailRocket.Application.Services.Shop;
using RetailRocket.Application.Services.Security;
using RetailRocket.Domain.Entities.Shop;

namespace RetailRocket.API.Controllers.Shop;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    
    public UserController(UserService userService) =>
        _userService = userService;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllUsersAsync();
        var result = users.Select(u => new UserResponseDto
        {
            UserId = u.UserId,
            Username = u.Username,
            Email = u.Email,
            CreatedAt = u.CreatedAt
        });
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var user = await _userService.GetUserAsync(id);
        if (user is null) return NotFound();
        return Ok(new UserResponseDto
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        });
    }

    [HttpGet("by-username")]
    public async Task<IActionResult> GetByUsername([FromQuery] string username)
    {
        var user = await _userService.GetUserByUsernameAsync(username);
        if (user is null) return NotFound();
        return Ok(new UserResponseDto
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        });
    }
    
    [HttpGet("by-email")]
    public async Task<IActionResult> GetByEmail([FromQuery] string email)
    {
        var user = await _userService.GetUserByEmailAsync(email);
        if (user is null) return NotFound();
        return Ok(new UserResponseDto
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UserRequestDto requestDto)
    {
        if (requestDto.Username is null || requestDto.Email is null || requestDto.Password is null)
            return BadRequest("Username, email and password are required.");
        
        var existingUser = await _userService.GetUserByUsernameAsync(requestDto.Username);
        if (existingUser is not null) return Conflict("Username already exists.");
        
        var existingEmail = await _userService.GetUserByEmailAsync(requestDto.Email);
        if (existingEmail is not null) return Conflict("Email already exists.");
        
        var hash = PasswordHasher.Hash(requestDto.Password);
        var user = new User(requestDto.Username, requestDto.Email, hash);
        await _userService.AddUserAsync(user);
        
        return CreatedAtAction(nameof(GetById), new { id = user.UserId }, new UserResponseDto
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UserRequestDto requestDto)
    {
        var user = await _userService.GetUserAsync(id);
        if (user is null) return NotFound();
        user.UpdateUsername(requestDto.Username);
        user.UpdateEmail(requestDto.Email);
        await _userService.UpdateUserAsync(user);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var user = await _userService.GetUserAsync(id);
        if (user is null) return NotFound();
        await _userService.DeleteUserAsync(id);
        return NoContent();
    }
}