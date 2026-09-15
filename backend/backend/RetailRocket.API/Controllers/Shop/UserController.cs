using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailRocket.Application.DTOs.Request.Shop;
using RetailRocket.Application.DTOs.Response.Shop;
using RetailRocket.Application.Services.Shop;
using RetailRocket.Application.Services.Security;
using RetailRocket.Application.Services.JWT;
using RetailRocket.Domain.Entities.Shop;

namespace RetailRocket.API.Controllers.Shop;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    private readonly JwtTokenService _tokenService;
    private readonly IMapper _mapper;

    public UserController(UserService userService, JwtTokenService tokenService, IMapper mapper)
    {
        _userService = userService;
        _tokenService = tokenService;
        _mapper = mapper;
    }

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

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        if (dto.Email is null || dto.Password is null)
            return BadRequest("Email and password are required.");

        var user = await _userService.GetUserByEmailAsync(dto.Email);
        if (user is null || !PasswordHasherService.Verify(user.PasswordHash!, dto.Password))
            return Unauthorized("Invalid credentials");

        var token = _tokenService.GenerateToken(user);

        return Ok(new
        {
            token,
            expiresIn = 30,
            userId = user.UserId,
            username = user.Username
        });
    }
    
    [Authorize]
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