using Microsoft.Extensions.Configuration;
using Services.Interfaces;
using System.Net.Mail;
using System.Net;

public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public SmtpEmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string to, string subject, string htmlBody)
    {
        var smtpSection = _configuration.GetSection("Smtp");

        var smtpClient = new SmtpClient(smtpSection["Host"])
        {
            Port = int.Parse(smtpSection["Port"]),
            Credentials = new NetworkCredential(smtpSection["Username"], smtpSection["Password"]),
            EnableSsl = bool.Parse(smtpSection["EnableSsl"])
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(smtpSection["From"]),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };

        mailMessage.To.Add(to);

        await smtpClient.SendMailAsync(mailMessage);
    }
}
