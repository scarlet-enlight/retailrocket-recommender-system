namespace RetailRocket.Application.Services.Mail;

public static class EmailTemplates
{
    public static EmailContent PasswordReset(string username, string link)
    {
        string html = "";
        string text = "";

        return new EmailContent("Password Reset", html, text);
    }

    public static EmailContent Newsletter(string username, string link)
    {
        return new EmailContent("topic", "html", "text");
    }
}