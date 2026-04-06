using System.Net;
using System.Net.Http.Json;
using Fintrack.IntegrationTests.Infrastructure;
using Fintrack.Server.Api.Controllers.Expenses;
using Fintrack.Server.Domain.Expenses;
using Fintrack.Server.Infrastructure.Authorization;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Fintrack.IntegrationTests.Expenses;

/// <summary>
/// API authorization for expenses: default test users get <see cref="Roles.User"/> permissions via claims transformation,
/// so these tests use explicit integration roles (see <see cref="Roles.IntegrationTestNoExpenses"/> / <see cref="Roles.IntegrationTestExpenseReadOnly"/>).
/// </summary>
public sealed class ExpensesControllerAuthorizationTests : BaseIntegrationTest
{
    public ExpensesControllerAuthorizationTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    // ═══════════════════════════════════════════════════════════════
    // UNAUTHENTICATED (401)
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public async Task Get_Should_ReturnUnauthorized_When_NotAuthenticated()
    {
        RemoveAuthentication();

        var response = await GetAsync($"/api/v1/expenses/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Create_Should_ReturnUnauthorized_When_NotAuthenticated()
    {
        RemoveAuthentication();

        var request = new CreateExpenseRequest(
            "Store", 50m, DateTime.UtcNow, null, null,
            new List<RequestExpenseItemData> { new(Guid.NewGuid(), 50m, null) });

        var response = await PostAsync("/api/v1/expenses", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Update_Should_ReturnUnauthorized_When_NotAuthenticated()
    {
        RemoveAuthentication();

        var request = new UpdateExpenseRequest(
            50m, DateTime.UtcNow, null, null, null,
            new List<RequestExpenseItemData> { new(Guid.NewGuid(), 50m, null) });

        var response = await PutAsync($"/api/v1/expenses/{Guid.NewGuid()}", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Delete_Should_ReturnUnauthorized_When_NotAuthenticated()
    {
        RemoveAuthentication();

        var response = await DeleteAsync($"/api/v1/expenses/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // ═══════════════════════════════════════════════════════════════
    // FORBIDDEN — no expense permissions at all (403)
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public async Task Get_Should_ReturnForbidden_When_UserLacksExpenseReadPermission()
    {
        AuthenticateAs(
            userId: "no-expense-user",
            permissions: null,
            roles: new[] { Roles.IntegrationTestNoExpenses });

        var response = await GetAsync($"/api/v1/expenses/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Create_Should_ReturnForbidden_When_UserLacksExpenseWritePermission()
    {
        AuthenticateAs(
            userId: "no-expense-user",
            permissions: null,
            roles: new[] { Roles.IntegrationTestNoExpenses });

        var request = new CreateExpenseRequest(
            "Store", 50m, DateTime.UtcNow, null, null,
            new List<RequestExpenseItemData> { new(Guid.NewGuid(), 50m, null) });

        var response = await PostAsync("/api/v1/expenses", request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_Should_ReturnForbidden_When_UserLacksExpenseDeletePermission()
    {
        AuthenticateAs(
            userId: "no-expense-user",
            permissions: null,
            roles: new[] { Roles.IntegrationTestNoExpenses });

        var response = await DeleteAsync($"/api/v1/expenses/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // ═══════════════════════════════════════════════════════════════
    // READ-ONLY ROLE — can GET but cannot POST/PUT/DELETE (403)
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public async Task Get_Should_ReturnSuccessOrNotFound_When_UserHasExpenseReadOnly()
    {
        AuthenticateAs(
            userId: Guid.NewGuid().ToString(),
            permissions: null,
            roles: new[] { Roles.IntegrationTestExpenseReadOnly });

        var response = await GetAsync($"/api/v1/expenses/{Guid.NewGuid()}");

        // Not 401/403 — either 200 or 404 depending on data
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_Should_ReturnForbidden_When_UserHasOnlyExpenseRead()
    {
        AuthenticateAs(
            userId: Guid.NewGuid().ToString(),
            permissions: null,
            roles: new[] { Roles.IntegrationTestExpenseReadOnly });

        var request = new CreateExpenseRequest(
            "Store", 50m, DateTime.UtcNow, null, null,
            new List<RequestExpenseItemData> { new(Guid.NewGuid(), 50m, null) });

        var response = await PostAsync("/api/v1/expenses", request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        DbContext.ChangeTracker.Clear();
        var count = await DbContext.Expenses.CountAsync();
        count.Should().Be(0);
    }

    [Fact]
    public async Task Update_Should_ReturnForbidden_When_UserHasOnlyExpenseRead()
    {
        var ownerId = Guid.NewGuid().ToString();
        AuthenticateAs(ownerId, new[] { Permissions.ExpensesRead, Permissions.ExpensesWrite });

        var expenseId = await CreateTestExpenseForOwner();

        RemoveAuthentication();
        AuthenticateAs(
            userId: ownerId,
            permissions: null,
            roles: new[] { Roles.IntegrationTestExpenseReadOnly });

        var updateRequest = new UpdateExpenseRequest(
            999m, DateTime.UtcNow, "Hijack", null, null,
            new List<RequestExpenseItemData> { new(Guid.NewGuid(), 999m, null) });

        var response = await PutAsync($"/api/v1/expenses/{expenseId}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_Should_ReturnForbidden_When_UserHasOnlyExpenseRead()
    {
        var ownerId = Guid.NewGuid().ToString();
        AuthenticateAs(ownerId, new[] { Permissions.ExpensesRead, Permissions.ExpensesWrite, Permissions.ExpensesDelete });

        var expenseId = await CreateTestExpenseForOwner();

        RemoveAuthentication();
        AuthenticateAs(
            userId: ownerId,
            permissions: null,
            roles: new[] { Roles.IntegrationTestExpenseReadOnly });

        var response = await DeleteAsync($"/api/v1/expenses/{expenseId}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // ═══════════════════════════════════════════════════════════════
    // HELPERS
    // ═══════════════════════════════════════════════════════════════

    private async Task<Guid> CreateTestExpenseForOwner()
    {
        var request = new CreateExpenseRequest(
            Merchant: "Auth Test Store",
            TotalAmount: 40.00m,
            Date: DateTime.UtcNow,
            InvoiceNumber: null,
            InvoiceImageUrl: null,
            Items: new List<RequestExpenseItemData>
            {
                new(Guid.NewGuid(), 40.00m, "Auth item")
            });

        var response = await PostAsync("/api/v1/expenses", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        return await response.Content.ReadFromJsonAsync<Guid>();
    }
}
