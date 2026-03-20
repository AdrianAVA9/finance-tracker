using System.Net;
using System.Net.Http.Json;
using Fintrack.IntegrationTests.Infrastructure;
using Fintrack.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using NSubstitute;

namespace Fintrack.IntegrationTests.Email;

[Collection("Integration")]
public class ForgotPasswordTests : BaseIntegrationTest
{
    // Shortcut to the factory's shared mock so we can assert on it
    private readonly IEmailSender<ApplicationUser> _emailSender;

    public ForgotPasswordTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        _emailSender = factory.MockEmailSender;
        // Clear any previous call records between tests
        _emailSender.ClearReceivedCalls();
    }

        // ── /forgotPassword ─────────────────────────────────────────────────────

    [Fact]
    public async Task ForgotPassword_WithConfirmedEmail_ShouldReturn200AndSendEmail()
    {
        // Arrange — create a confirmed user
        await SeedConfirmedUserAsync("confirmed@test.com", "Test1234!");

        // Act
        var response = await PostAsync("/forgotPassword", new { email = "confirmed@test.com" });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        await _emailSender.Received(1).SendPasswordResetCodeAsync(
            Arg.Any<ApplicationUser>(),
            "confirmed@test.com",
            Arg.Any<string>());
    }

    [Fact]
    public async Task ForgotPassword_WithUnconfirmedEmail_ShouldReturn200ButNotSendEmail()
    {
        // Arrange — create user WITHOUT confirming email
        await SeedUnconfirmedUserAsync("unconfirmed@test.com", "Test1234!");

        // Act
        var response = await PostAsync("/forgotPassword", new { email = "unconfirmed@test.com" });

        // Assert — 200 OK (Identity never reveals whether the user exists)
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        await _emailSender.DidNotReceive().SendPasswordResetCodeAsync(
            Arg.Any<ApplicationUser>(),
            Arg.Any<string>(),
            Arg.Any<string>());
    }

    [Fact]
    public async Task ForgotPassword_WithNonExistentEmail_ShouldReturn200AndNotSendEmail()
    {
        // Act
        var response = await PostAsync("/forgotPassword", new { email = "ghost@test.com" });

        // Assert — 200 OK regardless (prevents user enumeration)
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        await _emailSender.DidNotReceive().SendPasswordResetCodeAsync(
            Arg.Any<ApplicationUser>(),
            Arg.Any<string>(),
            Arg.Any<string>());
    }

    [Fact]
    public async Task ForgotPassword_WithInvalidEmailFormat_ShouldReturn400()
    {
        // Act
        var response = await PostAsync("/forgotPassword", new { email = "not-an-email" });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ── Helpers ─────────────────────────────────────────────────────────────

    private async Task SeedConfirmedUserAsync(string email, string password)
    {
        using var scope = Factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var user = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true };
        var result = await userManager.CreateAsync(user, password);
        result.Succeeded.Should().BeTrue(because: string.Join(", ", result.Errors.Select(e => e.Description)));
    }

    private async Task SeedUnconfirmedUserAsync(string email, string password)
    {
        using var scope = Factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var user = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = false };
        var result = await userManager.CreateAsync(user, password);
        result.Succeeded.Should().BeTrue(because: string.Join(", ", result.Errors.Select(e => e.Description)));
    }
}
