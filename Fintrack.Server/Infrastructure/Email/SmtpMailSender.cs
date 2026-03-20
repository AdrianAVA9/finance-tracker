using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace Fintrack.Server.Infrastructure.Email
{
    /// <summary>
    /// Simple SMTP email sender driven by appsettings "Smtp" section.
    /// For production, swap this out for SendGrid, Mailgun, etc.
    /// Example appsettings.Development.json:
    /// {
    ///   "App": { "FrontendUrl": "https://localhost:5173" },
    ///   "Smtp": {
    ///     "Host": "smtp.gmail.com",
    ///     "Port": 587,
    ///     "EnableSsl": true,
    ///     "FromEmail": "noreply@fintrack.app",
    ///     "FromName": "Fintrack",
    ///     "Username": "you@gmail.com",
    ///     "Password": "your-app-password"
    ///   }
    /// }
    /// </summary>
    public class SmtpMailSender : IEmailSender
    {
        private readonly SmtpConfig _cfg;

        public SmtpMailSender(IConfiguration config)
        {
            _cfg = config.GetSection("Smtp").Get<SmtpConfig>() ?? new SmtpConfig();
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            if (string.IsNullOrWhiteSpace(_cfg.Host))
                return; // No SMTP configured — silently skip (log in production)

            using var client = new SmtpClient(_cfg.Host, _cfg.Port)
            {
                EnableSsl = _cfg.EnableSsl,
                Credentials = new NetworkCredential(_cfg.Username, _cfg.Password)
            };

            var mail = new MailMessage
            {
                From = new MailAddress(_cfg.FromEmail, _cfg.FromName),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };
            mail.To.Add(email);

            await client.SendMailAsync(mail);
        }

        private sealed class SmtpConfig
        {
            public string Host { get; init; } = "";
            public int Port { get; init; } = 587;
            public bool EnableSsl { get; init; } = true;
            public string FromEmail { get; init; } = "noreply@fintrack.app";
            public string FromName { get; init; } = "Fintrack";
            public string Username { get; init; } = "";
            public string Password { get; init; } = "";
        }
    }
}
