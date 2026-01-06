using HMSSystem.Service;
using System.Net;
using System.Net.Mail;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
    {
        var smtp = _config.GetSection("SmtpSettings");

        var client = new SmtpClient(smtp["Host"], int.Parse(smtp["Port"]))
        {
            Credentials = new NetworkCredential(
                smtp["Username"],
                smtp["Password"]
            ),
            EnableSsl = bool.Parse(smtp["EnableSsl"])
        };

        var mail = new MailMessage
        {
            From = new MailAddress(smtp["FromEmail"], smtp["FromName"]),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };

        mail.To.Add(toEmail);

        await client.SendMailAsync(mail);
    }
}
