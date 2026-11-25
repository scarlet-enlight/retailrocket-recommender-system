namespace RetailRocket.Domain.Entities.Shop;

public class User
{
    public Guid user_id { get; }
    public string username { get; private set; }
    public string email { get; private set; }
    public string password_hash { get; private set; }
    public short role_level { get; private set; }
    public DateTime created_at { get; }
    
    public User(string username, string email, string password_hash)
    {
        this.username = username;
        this.email = email;
        this.password_hash = password_hash;
    }
    
    public void UpdateEmail(string email) => this.email = email;
    public void UpdateUsername(string username) => this.username = username;

    public void UpdatePassword(string password_hash) => this.password_hash = password_hash;
}