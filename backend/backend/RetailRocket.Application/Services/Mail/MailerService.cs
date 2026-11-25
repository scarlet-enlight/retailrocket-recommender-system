using MailKit;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using RetailRocket.Application.Interfaces.Mail;
using RetailRocket.Domain.Exceptions;

namespace RetailRocket.Application.Services.Mail;

public class MailerService : IMailerService
{
    private readonly SmtpSettings _settings;

    public MailerService(IOptions<SmtpSettings> settings) =>
        _settings = settings.Value;
    

    public async Task SendEmailAsync(string to, string subject, string htmlBody, string textBody)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            var builder = new BodyBuilder();
            
            builder.TextBody = textBody;
            builder.HtmlBody = htmlBody;
            
            message.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_settings.Server, _settings.Port, SecureSocketOptions.StartTls);
                await client.SendAsync(message);
                await client.AuthenticateAsync(_settings.Username, _settings.Password);
                await client.DisconnectAsync(true);
            }
        }
        catch (Exception e)
        {
            throw new EmailDeliveryException("SMTP server is down. Please wait until the problem's been fixed.");
        }
    }
}