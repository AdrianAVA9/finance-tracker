using System.Net;
using System.Net.Http.Json;
using Fintrack.IntegrationTests.Infrastructure;
using Fintrack.Server.Api.Controllers.Incomes;
using Fintrack.Server.Infrastructure.Authorization;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Fintrack.IntegrationTests.Incomes;

/// <summary>
/// Authorization for incomes endpoints using integration-only roles (see <see cref="Roles"/>).
/// </summary>
public sealed class IncomesControllerAuthorizationTests : BaseIntegrationTest
{
    public IncomesControllerAuthorizationTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GetById_Should_ReturnUnauthorized_When_NotAuthenticated()
    {
        RemoveAuthentication();

        var response = await GetAsync($"/api/v1/incomes/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetById_Should_ReturnForbidden_When_UserLacksIncomeReadPermission()
    {
        AuthenticateAs(
            userId: "no-income-user",
            permissions: null,
            roles: new[] { Roles.IntegrationTestNoIncomes });

        var response = await GetAsync($"/api/v1/incomes/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Create_Should_ReturnForbidden_When_UserHasOnlyIncomeRead()
    {
        AuthenticateAs(
            userId: Guid.NewGuid().ToString(),
            permissions: null,
            roles: new[] { Roles.IntegrationTestIncomeReadOnly });

        var categoryId = await DbContext.IncomeCategories.Select(c => c.Id).FirstAsync();
        var request = new CreateIncomeRequest(
            "X",
            10m,
            categoryId,
            DateTime.UtcNow.Date,
            null,
            false,
            null,
            null);

        var response = await PostAsync("/api/v1/incomes", request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        DbContext.ChangeTracker.Clear();
        (await DbContext.Incomes.CountAsync()).Should().Be(0);
    }
}
