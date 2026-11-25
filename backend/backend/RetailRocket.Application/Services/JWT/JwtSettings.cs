using System.ComponentModel.DataAnnotations;

namespace RetailRocket.Application.Services.JWT;

public class JwtSettings
{
    public const string SectionName = "Jwt";

    [Required, MinLength(32)] public string Key { get; set; } = "";
    [Required] public string Issuer { get; set; } = "";
    [Required] public string Audience { get; set; } = "";
    [Range(1, 1440)] public int ExpiryMinutesDefault { get; set; }
    [Range(1, 43200)] public int ExpiryMinutesRememberMe { get; set; }
}