using Fintrack.Server.Infrastructure.Email;
using Fintrack.Server.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace Fintrack.Tests.Infrastructure.Email;

public class IdentityEmailSenderTests
{
    private readonly IEmailSender _smtpSender = Substitute.For<IEmailSender>();
    private readonly ApplicationUser _user = new() { Email = "user@test.com" };

    private IdentityEmailSender CreateSender(string frontendUrl = "https://localhost:5173")
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["App:FrontendUrl"] = frontendUrl
            })
            .Build();

        return new IdentityEmailSender(_smtpSender, configuration);
    }

    // ── SendPasswordResetCodeAsync ──────────────────────────────────────────

    [Fact]
    public async Task SendPasswordResetCodeAsync_ShouldBuildCorrectResetLink()
    {
        // Arrange
        var sender = CreateSender("https://app.fintrack.io");
        string? capturedHtml = null;
        await _smtpSender.SendEmailAsync(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Do<string>(html => capturedHtml = html));

        // Act
        await sender.SendPasswordResetCodeAsync(_user, "user@test.com", "RESET_CODE_123");

        // Assert — link must point to the frontend reset page with correct params
        capturedHtml.Should().Contain("https://app.fintrack.io/auth/reset-password");
        capturedHtml.Should().Contain("email=user%40test.com");
        capturedHtml.Should().Contain("resetCode=RESET_CODE_123");
    }

    [Fact]
    public async Task SendPasswordResetCodeAsync_ShouldSendToCorrectRecipient()
    {
        // Arrange
        var sender = CreateSender();

        // Act
        await sender.SendPasswordResetCodeAsync(_user, "user@test.com", "CODE");

        // Assert
        await _smtpSender.Received(1).SendEmailAsync(
            "user@test.com",
            Arg.Any<string>(),
            Arg.Any<string>());
    }

    [Fact]
    public async Task SendPasswordResetCodeAsync_ShouldUrlEncodeSpecialCharactersInResetCode()
    {
        // Arrange — reset codes from Identity often contain '+' and '/' which must be encoded
        var sender = CreateSender();
        string? capturedHtml = null;
        await _smtpSender.SendEmailAsync(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Do<string>(html => capturedHtml = html));

        // Act
        await sender.SendPasswordResetCodeAsync(_user, "user@test.com", "code+with/special==");

        // Assert — '+' should be encoded as %2B, '/' as %2F
        capturedHtml.Should().NotContain("code+with/special==");
        capturedHtml.Should().Contain("code%2Bwith%2Fspecial%3D%3D");
    }

    // ── SendPasswordResetLinkAsync ──────────────────────────────────────────

    [Fact]
    public async Task SendPasswordResetLinkAsync_ShouldForwardLinkToSmtpSender()
    {
        // Arrange
        var sender = CreateSender();
        var resetLink = "https://app.fintrack.io/auth/reset-password?email=user%40test.com&resetCode=ABC";

        // Act
        await sender.SendPasswordResetLinkAsync(_user, "user@test.com", resetLink);

        // Assert
        await _smtpSender.Received(1).SendEmailAsync(
            "user@test.com",
            Arg.Is<string>(s => s.Contains("Reset")),
            Arg.Is<string>(html => html.Contains(resetLink)));
    }

    // ── SendConfirmationLinkAsync ───────────────────────────────────────────

    [Fact]
    public async Task SendConfirmationLinkAsync_ShouldSendConfirmationEmail()
    {
        // Arrange
        var sender = CreateSender();

        // Act
        await sender.SendConfirmationLinkAsync(_user, "user@test.com", "https://confirm.link");

        // Assert
        await _smtpSender.Received(1).SendEmailAsync(
            "user@test.com",
            Arg.Is<string>(s => s.Contains("Confirm")),
            Arg.Is<string>(html => html.Contains("https://confirm.link")));
    }

    [Fact]
    public async Task SendConfirmationLinkAsync_ShouldHtmlEncodeTheLink()
    {
        // Arrange
        var sender = CreateSender();
        string? capturedHtml = null;
        await _smtpSender.SendEmailAsync(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Do<string>(html => capturedHtml = html));

        // Act — passing a link with characters that need HTML encoding
        await sender.SendConfirmationLinkAsync(_user, "user@test.com", "https://confirm.link?token=A&B=C");

        // Assert — '&' should be HTML-encoded as &amp; to prevent XSS
        capturedHtml.Should().Contain("https://confirm.link?token=A&amp;B=C");
    }
}
