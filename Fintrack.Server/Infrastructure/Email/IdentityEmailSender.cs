using Fintrack.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Text.Encodings.Web;

namespace Fintrack.Server.Infrastructure.Email
{
    /// <summary>
    /// Bridges ASP.NET Core Identity's IEmailSender<TUser> to send password reset emails
    /// via the configured IEmailSender service (e.g. SMTP, SendGrid, etc.).
    /// </summary>
    public class IdentityEmailSender : IEmailSender<ApplicationUser>
    {
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _config;

        public IdentityEmailSender(IEmailSender emailSender, IConfiguration config)
        {
            _emailSender = emailSender;
            _config = config;
        }

        public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
            => _emailSender.SendEmailAsync(email, "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(confirmationLink)}'>clicking here</a>.");

        public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
            => _emailSender.SendEmailAsync(email, "Reset your Fintrack password",
                BuildResetEmail(email, resetLink));

        public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
        {
            // Construct the frontend URL manually when Identity provides just the code
            var frontendBase = _config["App:FrontendUrl"] ?? "https://localhost:5173";
            var encodedEmail = Uri.EscapeDataString(email);
            var encodedCode = Uri.EscapeDataString(resetCode);
            var resetLink = $"{frontendBase}/auth/reset-password?email={encodedEmail}&resetCode={encodedCode}";
            return SendPasswordResetLinkAsync(user, email, resetLink);
        }

        private static string BuildResetEmail(string email, string resetLink) =>
            $"""
            <!DOCTYPE html>
            <html>
            <body style="font-family:sans-serif;background:#0B0E11;color:#e2e8f0;padding:40px;">
              <div style="max-width:480px;margin:auto;background:#1e293b;border-radius:12px;padding:32px;border:1px solid #334155;">
                <h2 style="color:#38bdf8;margin:0 0 8px 0;">Fintrack — Password Reset</h2>
                <p>Hi, we received a request to reset the password for <strong>{email}</strong>.</p>
                <p>Click the button below to choose a new password:</p>
                <a href="{resetLink}"
                   style="display:inline-block;background:#1e6a7b;color:#fff;font-weight:700;
                          padding:14px 28px;border-radius:8px;text-decoration:none;margin:16px 0;">
                  Reset Password
                </a>
                <p style="color:#94a3b8;font-size:13px;">
                  This link expires in 24 hours.<br/>
                  If you didn't request this, you can safely ignore this email.
                </p>
              </div>
            </body>
            </html>
            """;
    }
}
