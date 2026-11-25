namespace RetailRocket.Application.Services.Mail;

public sealed record EmailContent
{
    public string Subject { get; set; }
    public string Html { get; set; }
    public string Text { get; set; }
    
    public EmailContent(string Subject, string Html, string Text)
    {
        this.Subject = Subject;
        this.Html = Html;
        this.Text = Text;
    }
}