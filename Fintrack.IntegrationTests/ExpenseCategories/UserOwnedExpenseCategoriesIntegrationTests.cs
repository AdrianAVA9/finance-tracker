using System.Net;
using System.Net.Http.Json;
using Fintrack.Server.Api.Controllers.ExpenseCategories;
using Fintrack.IntegrationTests.Infrastructure;
using Fintrack.Server.Infrastructure.Authorization;
using FluentAssertions;
using Xunit;

namespace Fintrack.IntegrationTests.ExpenseCategories;

[Collection("Integration")]
public sealed class UserOwnedExpenseCategoriesIntegrationTests : BaseIntegrationTest
{
    public UserOwnedExpenseCategoriesIntegrationTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GetOwned_Should_Return401_When_NotAuthenticated()
    {
        RemoveAuthentication();

        var response = await GetAsync("/api/v1/expensecategories/owned");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetOwned_Should_ReturnOnlyRowsForCurrentUser()
    {
        var testUserId = "user-owned-exp-" + Guid.NewGuid().ToString("N")[..12];
        AuthenticateAs(testUserId, new[] { Permissions.CategoriesRead, Permissions.CategoriesWrite });

        var createReq = new RequestCreateExpenseCategory
        {
            Name = "UserOwnedOnly",
            Icon = "shopping_bag",
            Color = "#00FF00"
        };
        var post = await PostAsync("/api/v1/expensecategories", createReq);
        post.StatusCode.Should().Be(HttpStatusCode.Created);

        var ownedResp = await GetAsync("/api/v1/expensecategories/owned");
        ownedResp.StatusCode.Should().Be(HttpStatusCode.OK);
        var owned = await ownedResp.Content.ReadFromJsonAsync<List<ExpenseCategoryOwnedJson>>();
        owned.Should().NotBeNull();
        owned!.Should().NotBeEmpty();
        owned.Should().OnlyContain(c => c.UserId == testUserId);
        owned.Should().Contain(c => c.Name == "UserOwnedOnly");
    }

    private sealed class ExpenseCategoryOwnedJson
    {
        public Guid Id { get; init; }
        public string? Name { get; init; }
        public string? UserId { get; init; }
    }
}
