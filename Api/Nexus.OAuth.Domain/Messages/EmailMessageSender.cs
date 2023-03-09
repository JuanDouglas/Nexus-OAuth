using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;

namespace Nexus.OAuth.Domain.Messages;
public class EmailMessageSender
{
    private const string section = "Mail";

    public bool UseSmtp { get; set; }
    public SendGrid SendGrid { get; set; }
    public Smtp Smtp { get; set; }
    public EmailMessageSender(IConfiguration config)
    {
        var sec = config.GetSection(section);

        SendGrid = config.GetSection(nameof(SendGrid)).Get<SendGrid>();
        Smtp = sec.GetSection(nameof(Smtp)).Get<Smtp>();
        UseSmtp = sec.GetSection(nameof(UseSmtp)).Get<bool>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="htmlContent"></param>
    /// <param name="subject"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    /// 
    public async Task SendEmailAsync(string htmlContent, string subject, string to, string from = "no-reply@mail.nexus-company.net")
    {
        if (!UseSmtp)
            await SendWithSendGridAsync(htmlContent, subject, to);

        await SendWithSmtpAsync(htmlContent, subject, to, from);
    }

    private async Task SendWithSendGridAsync(string htmlContent, string subject, string to)
    {
        var client = new SendGridClient(SendGrid.ApiKey);
        var from_email = new EmailAddress(SendGrid.SenderEmail, SendGrid.SenderName);
        var to_email = new EmailAddress(to);
        var msg = MailHelper.CreateSingleEmail(from_email, to_email, subject, string.Empty, htmlContent);

        await client.SendEmailAsync(msg);
    }

    private async Task SendWithSmtpAsync(string htmlContent, string subject, string to, string from)
    {
        try
        {
            MailMessage mail = new();
            using SmtpClient SmtpServer = new(Smtp.Host);

            mail.From = new MailAddress(from);
            mail.To.Add(to);
            mail.Subject = subject;
            mail.Body = htmlContent;
            mail.IsBodyHtml = true;

            SmtpServer.Credentials = new System.Net.NetworkCredential(Smtp.Login, Smtp.Password);
            SmtpServer.EnableSsl = Smtp.EnableSsl;
            SmtpServer.Port = Smtp.Port;
            SmtpServer.UseDefaultCredentials = false;

            SmtpServer.Send(mail);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            throw ex;
        }
    }
}

public class SendGrid
{
    public string SenderEmail { get; set; }
    public string SenderName { get; set; }
    public string ApiKey { get; set; }
}

public class Smtp
{
    public string Host { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public bool EnableSsl { get; set; }
    public int Port { get; set; }
}