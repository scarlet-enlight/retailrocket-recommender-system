using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RetailRocket.Domain.Entities.Shop;

namespace RetailRocket.Application.Services.JWT;

public class JwtTokenService
{
    private readonly JwtSettings _settings;

    public JwtTokenService(IOptions<JwtSettings> options) =>
        _settings = options.Value;

    public IssuedToken GenerateToken(User user, bool rememberMe)
    {
        var minutes = rememberMe ? _settings.ExpiryMinutesRememberMe : _settings.ExpiryMinutesDefault;
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(minutes);
        
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_settings.Key));
        
        var credentials = new SigningCredentials(
            key, SecurityAlgorithms.HmacSha256);
        
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.user_id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.email!),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.username!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: expiresAt.UtcDateTime,
            signingCredentials: credentials
        );
        
        return new IssuedToken(new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}