using System.Net;
using System.Net.Http.Json;
using Fintrack.IntegrationTests.Infrastructure;
using Fintrack.Server.Application.Budgets.Commands.UpsertBudgets;
using Fintrack.Server.Api.Controllers.Budgets;
using Fintrack.Server.Domain.Budgets;
using Fintrack.Server.Domain.ExpenseCategories;
using Fintrack.Server.Infrastructure.Authorization;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Fintrack.IntegrationTests.Budgets;

/// <summary>
/// API authorization for budgets: default test users get <see cref="Roles.User"/> permissions via claims transformation,
/// so these tests use explicit integration roles (see <see cref="Roles.IntegrationTestNoBudgets"/>).
/// </summary>
public sealed class BudgetsControllerAuthorizationTests : BaseIntegrationTest
{
    public BudgetsControllerAuthorizationTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Get_Should_ReturnUnauthorized_When_NotAuthenticated()
    {
        RemoveAuthentication();

        var response = await GetAsync("/api/v1/budgets?month=1&year=2024");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Get_Should_ReturnForbidden_When_UserLacksBudgetReadPermission()
    {
        AuthenticateAs(
            userId: "no-budget-user",
            permissions: null,
            roles: new[] { Roles.IntegrationTestNoBudgets });

        var response = await GetAsync("/api/v1/budgets?month=1&year=2024");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Get_Should_ReturnOk_When_UserHasBudgetReadOnlyRole()
    {
        AuthenticateAs(
            userId: Guid.NewGuid().ToString(),
            permissions: null,
            roles: new[] { Roles.IntegrationTestBudgetReadOnly });

        var response = await GetAsync("/api/v1/budgets?month=1&year=2024");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task BatchUpsert_Should_ReturnForbidden_When_UserHasOnlyBudgetRead()
    {
        var userId = Guid.NewGuid().ToString();
        AuthenticateAs(
            userId,
            permissions: null,
            roles: new[] { Roles.IntegrationTestBudgetReadOnly });

        var group = new ExpenseCategoryGroup { Name = "G" };
        await AddAsync(group);
        var category = ExpenseCategoryTestHelpers.CreateWithGroup(group, name: "C");
        await AddAsync(category);

        var request = new UpsertBudgetsRequest(1, 2024, new List<BudgetEntryDto> { new(category.Id, 50m) });

        var response = await PostAsync("/api/v1/budgets/batch", request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        DbContext.ChangeTracker.Clear();
        var count = await DbContext.Budgets.CountAsync(b => b.UserId == userId);
        count.Should().Be(0);
    }

    [Fact]
    public async Task BatchUpsert_Should_ReturnUnauthorized_When_NotAuthenticated()
    {
        RemoveAuthentication();

        var request = new UpsertBudgetsRequest(1, 2024, new List<BudgetEntryDto>());

        var response = await PostAsync("/api/v1/budgets/batch", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Delete_Should_ReturnForbidden_When_UserLacksBudgetWritePermission()
    {
        var userId = Guid.NewGuid().ToString();
        var group = new ExpenseCategoryGroup { Name = "G" };
        await AddAsync(group);
        var category = ExpenseCategoryTestHelpers.CreateWithGroup(group, name: "C");
        await AddAsync(category);

        var budget = Budget.Create(userId, category.Id, 10m, false, 2, 2024).Value;
        await AddAsync(budget);

        AuthenticateAs(
            userId,
            permissions: null,
            roles: new[] { Roles.IntegrationTestBudgetReadOnly });

        var response = await DeleteAsync($"/api/v1/budgets/{budget.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
