namespace RetailRocket.Domain.Entities.Shop;

public class User
{
    public Guid UserId { get; }
    public string? Username { get; private set; }
    public string? Email { get; private set; }
    public string? Password { get; private set; }
    public DateTime CreatedAt { get; }
    
    public User(string username, string email, string passwordHash)
    {
        Username = username;
        Email = email;
        Password = passwordHash;
    }
    
    public void UpdateEmail(string email) => Email = email;
    public void UpdateUsername(string username) => Username = username;
}