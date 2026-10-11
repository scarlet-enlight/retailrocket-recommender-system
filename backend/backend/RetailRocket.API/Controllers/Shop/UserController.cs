using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailRocket.Application.DTOs.Request.Shop.Users;
using RetailRocket.Application.DTOs.Response.Shop.Users;
using RetailRocket.Application.Services.Shop;
using RetailRocket.Application.Services.Security;
using RetailRocket.Application.Services.JWT;
using RetailRocket.Domain.Entities.Shop;
using RetailRocket.Domain.Exceptions;

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

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllUsersAsync();
        var result = _mapper.Map<IEnumerable<UserResponseDto>>(users);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        if (id == Guid.Empty)
            throw new ValidationException("User ID is required.");
        
        var user = await _userService.GetUserAsync(id);
        if (user is null)
            throw new NotFoundException("User not found.");
        return Ok(_mapper.Map<UserResponseDto>(user));
    }

    [AllowAnonymous]
    [HttpGet("by-username")]
    public async Task<IActionResult> GetByUsername([FromQuery] string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ValidationException("Username is required.");
        
        var user = await _userService.GetUserByUsernameAsync(username);
        if (user is null)
            throw new NotFoundException($"User {username} not found.");
        return Ok(_mapper.Map<UserResponseDto>(user));
    }
    
    [AllowAnonymous]
    [HttpGet("by-email")]
    public async Task<IActionResult> GetByEmail([FromQuery] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ValidationException("Email is required.");
        
        var user = await _userService.GetUserByEmailAsync(email);
        if (user is null) 
            throw new NotFoundException("User not found.");
        return Ok(_mapper.Map<UserResponseDto>(user));
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UserCreateRequestDto createRequestDto)
    {
        if (string.IsNullOrWhiteSpace(createRequestDto.Username)
            || string.IsNullOrWhiteSpace(createRequestDto.Email) 
            || string.IsNullOrWhiteSpace(createRequestDto.Password))
            throw new ValidationException("Username, email and password are required.");
        
        var existingUser = await _userService.GetUserByUsernameAsync(createRequestDto.Username);
        if (existingUser is not null) throw new ValidationException("Username already exists.");
        
        var existingEmail = await _userService.GetUserByEmailAsync(createRequestDto.Email);
        if (existingEmail is not null) throw new ValidationException("Email already exists.");
        
        var hash = PasswordHasherService.Hash(createRequestDto.Password);
        if (!PasswordHasherService.Verify(hash, createRequestDto.Password))
            throw new UnauthorizedAccessException("Something wrong has occured during signing in. Please sign in again.");
        
        var user = new User(createRequestDto.Username, createRequestDto.Email, hash);
        await _userService.AddUserAsync(user);

        var created = _userService.GetUserAsync(user.UserId);
        return CreatedAtAction(nameof(GetById), new { id = created.Id },
            _mapper.Map<UserResponseDto>(created));
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            throw new ValidationException("Email and password are required.");

        var user = await _userService.GetUserByEmailAsync(dto.Email);
        if (user is null || !PasswordHasherService.Verify(user.PasswordHash!, dto.Password))
            throw new AuthenticationException("Invalid credentials.");

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
    public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateRequestDto updateRequestDto)
    {
        if (id == Guid.Empty)
            throw new ValidationException("User ID is required.");
        
        var user = await _userService.GetUserAsync(id);
        if (user is null)
            throw new NotFoundException("User not found.");
        
        if (updateRequestDto.Username != user.Username && !string.IsNullOrWhiteSpace(updateRequestDto.Username)) 
            user.UpdateUsername(updateRequestDto.Username);
        if (updateRequestDto.Email != user.Email && !string.IsNullOrWhiteSpace(updateRequestDto.Email))
            user.UpdateEmail(updateRequestDto.Email);
        
        await _userService.UpdateUserAsync(user);
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (id == Guid.Empty)
            throw new ValidationException("User ID is required.");
        
        var user = await _userService.GetUserAsync(id);
        if (user is null) 
            throw new NotFoundException("User not found.");
        await _userService.DeleteUserAsync(id);
        return NoContent();
    }
}