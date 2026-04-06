using System.Net;
using System.Net.Http.Json;
using Fintrack.IntegrationTests.Infrastructure;
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
        AuthenticateAs(userId);

        var response = await GetAsync("/api/v1/incomes/categories");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await response.Content.ReadFromJsonAsync<List<IncomeCategoryResponse>>();
        list.Should().NotBeNull();
        list!.Should().NotBeEmpty();
        list.Should().Contain(c => c.Name == "Salario / Nómina" && c.IsEditable == false);
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
