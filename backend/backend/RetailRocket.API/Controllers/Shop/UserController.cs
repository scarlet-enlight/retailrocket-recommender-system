using System.Security.Claims;
using System.Security.Cryptography;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using RetailRocket.Application.DTOs.Request.Shop.Users;
using RetailRocket.Application.DTOs.Response.Shop.Users;
using RetailRocket.Application.Interfaces.Mail;
using RetailRocket.Application.Services.HashGen;
using RetailRocket.Application.Services.Shop;
using RetailRocket.Application.Services.Security;
using RetailRocket.Application.Services.JWT;
using RetailRocket.Application.Services.Mail;
using RetailRocket.Application.Services.WebAccess;
using RetailRocket.Domain.Entities.Shop;
using RetailRocket.Domain.Exceptions;
using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;

namespace RetailRocket.API.Controllers.Shop;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    private readonly JwtTokenService _tokenService;
    private readonly IMailerService _mailerService;
    private readonly HashGeneratorService _hashService;
    private readonly WebAccessSettings _settings;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public UserController(
        UserService userService,
        JwtTokenService tokenService,
        IMailerService mailerService,
        HashGeneratorService hashService,
        WebAccessSettings settings,
        IMapper mapper, ILogger logger)
    {
        _userService = userService;
        _tokenService = tokenService;
        _mailerService = mailerService;
        _hashService = hashService;
        _settings = settings;
        _mapper = mapper;
        _logger = logger;
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
    [HttpGet("username")]
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
    [HttpGet("email")]
    public async Task<IActionResult> GetByEmail([FromQuery] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ValidationException("Email is required.");
        
        var user = await _userService.GetUserByEmailAsync(email);
        if (user is null) 
            throw new NotFoundException("User not found.");
        return Ok(_mapper.Map<UserResponseDto>(user));
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetByCurrentlyLoggedIn()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(idClaim, out var userId))
            throw new AuthenticationException("No user to log out. Redirecting to login page.");

        var user = await _userService.GetUserAsync(userId);
        if (user is null)
            throw new AuthenticationException("User not found. Redirecting to login page.");
        
        return Ok(new
        {
            UserId = user.user_id,
            Username = user.username
        });
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
        if (existingUser is not null) 
            throw new ValidationException("Username already exists.");
        
        var existingEmail = await _userService.GetUserByEmailAsync(createRequestDto.Email);
        if (existingEmail is not null) 
            throw new ValidationException("Email already exists.");
        
        var hash = PasswordHasherService.Hash(createRequestDto.Password);
        if (!PasswordHasherService.Verify(hash, createRequestDto.Password))
            throw new AuthenticationException("Something wrong has occured during signing in. Please sign in again.");
        
        var user = new User(createRequestDto.Username, createRequestDto.Email, hash);
        await _userService.AddUserAsync(user);

        var created = await _userService.GetUserAsync(user.user_id);
        return CreatedAtAction(nameof(GetById), new { id = created!.user_id },
            _mapper.Map<UserResponseDto>(created));
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            throw new ValidationException("Email and password are required.");

        var user = await _userService.GetUserByEmailAsync(dto.Email);
        if (user is null || !PasswordHasherService.Verify(user.password_hash, dto.Password))
            throw new AuthenticationException("Invalid credentials.");

        var issued = _tokenService.GenerateToken(user, dto.RememberMe);
        
        Response.Cookies.Append("access_token", issued.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Path = "/",
            Expires = dto.RememberMe ? issued.ExpiresAt : null
        });

        return Ok(new
        {
            UserId = user.user_id,
            Username = user.username
        });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(idClaim, out var userId))
            throw new AuthenticationException("No user to log out. Redirecting to login page.");

        var user = await _userService.GetUserAsync(userId);
        if (user is null)
            throw new AuthenticationException("User not found. Redirecting to login page.");
        
        Response.Cookies.Delete("access_token", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Path = "/"
        });
        return NoContent();
    }

    [AllowAnonymous]
    [EnableRateLimiting("mail-service")]
    [HttpPost("password-reset")]
    public async Task<IActionResult> RequestPasswordReset([FromBody] RequestPasswordResetDto dto)
    {
        if (dto.Email is null || string.IsNullOrWhiteSpace(dto.Email))
            throw new ValidationException("Email is required.");
        
        var user = await _userService.GetUserByEmailAsync(dto.Email);
        if (user is null)
            return Ok(new
            {
                message = "If an account exists for this address, we've sent an email."
            });
        
        var token = _hashService.GenerateToken();
        _hashService.HashData(token);
        // insert here token storing into temp database with time limit
        var link = $"{_settings.BaseUrl}/reset-password?token={token}";
        var mail = EmailTemplates.PasswordReset(user.username, link);

        try
        {
            await _mailerService.SendEmailAsync(user.email, mail.Subject, mail.Html, mail.Text);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Password reset email failed for user {user_id}.", user.user_id);
        }

        return Ok(new
        {
            message = "If an account exists for this address, we've sent an email."
        });
    }
    
    // WIP: insert here sending email when password is forgotten
    
    [Authorize]
    [HttpPost("auth/password")]
    public async Task<IActionResult> ChangePassword(Guid id, [FromBody] PasswordChangeRequestDto passwordRequestDto)
    {
        if (id == Guid.Empty)
            throw new ValidationException("User ID is required.");
        
        var user = await _userService.GetUserAsync(id);
        if (user is null)
            throw new NotFoundException("User not found.");

        if (string.IsNullOrWhiteSpace(passwordRequestDto.OldPassword) ||
            string.IsNullOrWhiteSpace(passwordRequestDto.NewPassword) ||
            string.IsNullOrWhiteSpace(passwordRequestDto.RepeatPassword))
            throw new ValidationException("Old password and new passwords are required.");
        
        if (passwordRequestDto.NewPassword != passwordRequestDto.RepeatPassword)
            throw new ValidationException("New passwords must be the same.");
        
        // WIP: insert here check of old passwords history with new one
        
        var hash = PasswordHasherService.Hash(passwordRequestDto.NewPassword);
        if (!PasswordHasherService.Verify(hash, passwordRequestDto.NewPassword))
            throw new AuthenticationException("Something wrong has occured during changing this account's password. Please try again.");

        user.UpdatePassword(hash);
        await _userService.UpdateUserAsync(user);
        return NoContent();
    }
    
    // WIP: insert here password resetting when sent request via email
    
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateRequestDto updateRequestDto)
    {
        if (id == Guid.Empty)
            throw new ValidationException("User ID is required.");
        
        var user = await _userService.GetUserAsync(id);
        if (user is null)
            throw new NotFoundException("User not found.");

        if (updateRequestDto.Username == user.username)
            throw new ValidationException("You cannot change your username.");
        
        if (!string.IsNullOrWhiteSpace(updateRequestDto.Username)) 
            user.UpdateUsername(updateRequestDto.Username);
        if (!string.IsNullOrWhiteSpace(updateRequestDto.Email))
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