namespace RetailRocket.Application.Interfaces.Mail;

public interface IMailerService
{
    Task SendEmailAsync(string to, string subject, string htmlBody, string textBody);
}