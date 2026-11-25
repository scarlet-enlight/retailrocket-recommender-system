using System.Security.Cryptography;
using System.Text;

namespace RetailRocket.Application.Services.HashGen;

public class HashGeneratorService
{
    public string GenerateToken() =>
        Convert.ToHexStringLower(RandomNumberGenerator.GetBytes(32));
    
    public byte[] HashData(string token) =>
        SHA256.HashData(Encoding.UTF8.GetBytes(token));
}