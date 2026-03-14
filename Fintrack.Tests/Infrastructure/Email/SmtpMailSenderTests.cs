using Fintrack.Server.Infrastructure.Email;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace Fintrack.Tests.Infrastructure.Email;

public class SmtpMailSenderTests
{
    private static SmtpMailSender CreateSender(Dictionary<string, string?> config)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(config)
            .Build();
        return new SmtpMailSender(configuration);
    }

    [Fact]
    public async Task SendEmailAsync_WhenSmtpHostIsEmpty_ShouldSkipSilently()
    {
        // Arrange — no SMTP host configured
        var sender = CreateSender(new Dictionary<string, string?>
        {
            ["Smtp:Host"] = ""
        });

        // Act & Assert — should complete without throwing
        var act = async () => await sender.SendEmailAsync("user@test.com", "Test Subject", "<p>Hello</p>");
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task SendEmailAsync_WhenSmtpHostIsNull_ShouldSkipSilently()
    {
        // Arrange — Smtp section absent entirely
        var sender = CreateSender(new Dictionary<string, string?>());

        var act = async () => await sender.SendEmailAsync("user@test.com", "Test Subject", "<p>Hello</p>");
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task SendEmailAsync_WhenSmtpHostIsWhitespace_ShouldSkipSilently()
    {
        var sender = CreateSender(new Dictionary<string, string?>
        {
            ["Smtp:Host"] = "   "
        });

        var act = async () => await sender.SendEmailAsync("user@test.com", "Test Subject", "<p>Hello</p>");
        await act.Should().NotThrowAsync();
    }
}
