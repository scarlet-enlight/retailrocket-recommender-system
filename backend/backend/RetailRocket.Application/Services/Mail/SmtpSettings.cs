using System.ComponentModel.DataAnnotations;

namespace RetailRocket.Application.Services.Mail;

public class SmtpSettings
{
    public const string SectionName = "Smtp";
    
    [Required] public string Server { get; set; }
    [Required, Range(1, 65535)] public int Port { get; set; }
    [Required] public string SenderName { get; set; }
    [Required] public string SenderEmail { get; set; } 
    public string? Username { get; set; }
    public string? Password { get; set; }
}