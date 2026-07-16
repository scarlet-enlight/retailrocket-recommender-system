using Isopoh.Cryptography.Argon2;

namespace RetailRocket.Application.Services.Security;

public class PasswordHasher
{
    public static string Hash(string password) => 
        Argon2.Hash(password);
    
    public static bool Verify(string hash, string password) =>
        Argon2.Verify(hash, password);
}