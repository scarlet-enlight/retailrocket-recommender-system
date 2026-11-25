namespace RetailRocket.Application.Services.JWT;

public sealed record IssuedToken(string Token, DateTimeOffset ExpiresAt);