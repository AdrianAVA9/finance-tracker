using System.Net;
using System.Net.Http.Json;
using Fintrack.IntegrationTests.Infrastructure;
using Fintrack.Server.Domain.IncomeCategories;
using Fintrack.Server.Infrastructure.Authorization;
using FluentAssertions;
using Xunit;

namespace Fintrack.IntegrationTests.IncomeCategories;

[Collection("Integration")]
public sealed class IncomeCategoriesIntegrationTests : BaseIntegrationTest
{
    public IncomeCategoriesIntegrationTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GetCategories_Should_Return401_When_NotAuthenticated()
    {
        RemoveAuthentication();

        var response = await GetAsync("/api/v1/incomes/categories");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCategories_Should_ReturnSeededCategories_When_Authenticated()
    {
        var userId = Guid.NewGuid().ToString();
        AuthenticateAs(userId, new[] { Permissions.IncomesRead });

        var response = await GetAsync("/api/v1/incomes/categories");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await response.Content.ReadFromJsonAsync<List<IncomeCategoryResponse>>();
        list.Should().NotBeNull();
        list!.Should().NotBeEmpty();
        list.Should().Contain(c => c.Name == "Salario / Nómina" && c.IsEditable == false);
    }

    [Fact]
    public async Task GetUserOwnedCategories_Should_Return401_When_NotAuthenticated()
    {
        RemoveAuthentication();

        var response = await GetAsync("/api/v1/incomes/categories/owned");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetUserOwnedCategories_Should_ReturnEmpty_When_OnlySeededGlobalCategoriesExist()
    {
        var userId = Guid.NewGuid().ToString();
        AuthenticateAs(userId, new[] { Permissions.IncomesRead });

        var response = await GetAsync("/api/v1/incomes/categories/owned");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await response.Content.ReadFromJsonAsync<List<IncomeCategoryResponse>>();
        list.Should().NotBeNull();
        list!.Should().BeEmpty();
    }

    [Fact]
    public async Task GetUserOwnedCategories_Should_ReturnPersistedUserCategory()
    {
        var userId = Guid.NewGuid().ToString();
        var create = IncomeCategory.Create("Custom user income", "work", "#112233", userId, isEditable: true);
        create.IsSuccess.Should().BeTrue();
        DbContext.IncomeCategories.Add(create.Value);
        await DbContext.SaveChangesAsync();

        AuthenticateAs(userId, new[] { Permissions.IncomesRead });

        var response = await GetAsync("/api/v1/incomes/categories/owned");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await response.Content.ReadFromJsonAsync<List<IncomeCategoryResponse>>();
        list.Should().NotBeNull();
        list!.Should().Contain(c => c.Name == "Custom user income" && c.IsEditable);
    }

    public sealed class IncomeCategoryResponse
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? Icon { get; init; }
        public string? Color { get; init; }
        public bool IsEditable { get; init; }
    }
}
