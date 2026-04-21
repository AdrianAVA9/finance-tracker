using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Fintrack.IntegrationTests.Infrastructure;
using Fintrack.Server.Api.Controllers.Expenses;
using Fintrack.Server.Application.Expenses.Queries.GetExpenseById;
using Fintrack.Server.Domain.Expenses;
using Fintrack.Server.Infrastructure.Authorization;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Fintrack.IntegrationTests.Expenses;

public class ExpensesControllerTests : BaseIntegrationTest
{
    private static readonly string[] ExpenseUserPermissions =
    {
        Permissions.ExpensesRead,
        Permissions.ExpensesWrite,
        Permissions.ExpensesDelete
    };

    public ExpensesControllerTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    // ═══════════════════════════════════════════════════════════════
    // CREATE
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public async Task Create_Should_ReturnCreated_When_ValidExpense()
    {
        var userId = Guid.NewGuid().ToString();
        AuthenticateAs(userId, ExpenseUserPermissions);

        var categoryId = Guid.NewGuid();
        var request = new CreateExpenseRequest(
            Merchant: "Test Store",
            TotalAmount: 100.00m,
            Date: DateTime.UtcNow,
            InvoiceNumber: "INV-001",
            InvoiceImageUrl: null,
            Items: new List<RequestExpenseItemData>
            {
                new(categoryId, 100.00m, "Groceries")
            });

        var response = await PostAsync("/api/v1/expenses", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var newId = await response.Content.ReadFromJsonAsync<Guid>();
        newId.Should().NotBeEmpty();

        DbContext.ChangeTracker.Clear();
        var persisted = await DbContext.Expenses
            .Include(e => e.Items)
            .SingleOrDefaultAsync(e => e.Id == newId);
        persisted.Should().NotBeNull();
        persisted!.Merchant.Should().Be("Test Store");
        persisted.TotalAmount.Should().Be(100.00m);
        persisted.Items.Should().ContainSingle();
    }

    [Fact]
    public async Task Create_Should_PersistMultipleItems()
    {
        var userId = Guid.NewGuid().ToString();
        AuthenticateAs(userId, ExpenseUserPermissions);

        var catA = Guid.NewGuid();
        var catB = Guid.NewGuid();
        var request = new CreateExpenseRequest(
            Merchant: "Split Store",
            TotalAmount: 100.00m,
            Date: DateTime.UtcNow,
            InvoiceNumber: null,
            InvoiceImageUrl: null,
            Items: new List<RequestExpenseItemData>
            {
                new(catA, 60.00m, "Item 1"),
                new(catB, 40.00m, "Item 2")
            });

        var response = await PostAsync("/api/v1/expenses", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var newId = await response.Content.ReadFromJsonAsync<Guid>();

        DbContext.ChangeTracker.Clear();
        var items = await DbContext.ExpenseItems
            .Where(i => i.ExpenseId == newId)
            .ToListAsync();
        items.Should().HaveCount(2);
        items.Sum(i => i.ItemAmount).Should().Be(100.00m);
    }

    // ═══════════════════════════════════════════════════════════════
    // GET BY ID
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public async Task GetById_Should_ReturnOk_When_ExpenseExists()
    {
        var userId = Guid.NewGuid().ToString();
        AuthenticateAs(userId, ExpenseUserPermissions);

        var expenseId = await CreateTestExpense(userId);

        var response = await GetAsync($"/api/v1/expenses/{expenseId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await response.Content.ReadFromJsonAsync<ExpenseDetailsDto>();
        dto.Should().NotBeNull();
        dto!.Id.Should().Be(expenseId);
        dto.Merchant.Should().Be("Integration Store");
        dto.TotalAmount.Should().Be(75.00m);
    }

    [Fact]
    public async Task GetById_Should_ReturnNotFound_When_ExpenseDoesNotExist()
    {
        var userId = Guid.NewGuid().ToString();
        AuthenticateAs(userId, ExpenseUserPermissions);

        var response = await GetAsync($"/api/v1/expenses/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_Should_ReturnNotFound_When_ExpenseBelongsToAnotherUser()
    {
        var ownerId = Guid.NewGuid().ToString();
        AuthenticateAs(ownerId, ExpenseUserPermissions);
        var expenseId = await CreateTestExpense(ownerId);

        RemoveAuthentication();
        var otherId = Guid.NewGuid().ToString();
        AuthenticateAs(otherId, ExpenseUserPermissions);

        var response = await GetAsync($"/api/v1/expenses/{expenseId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ═══════════════════════════════════════════════════════════════
    // UPDATE
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public async Task Update_Should_ReturnOk_When_ValidRequest()
    {
        var userId = Guid.NewGuid().ToString();
        AuthenticateAs(userId, ExpenseUserPermissions);
        var expenseId = await CreateTestExpense(userId);

        var updateRequest = new UpdateExpenseRequest(
            TotalAmount: 120.00m,
            Date: new DateTime(2025, 7, 1),
            Merchant: "Updated Store",
            InvoiceNumber: "INV-002",
            InvoiceImageUrl: null,
            Items: new List<RequestExpenseItemData>
            {
                new(Guid.NewGuid(), 120.00m, "Updated item")
            });

        var response = await PutAsync($"/api/v1/expenses/{expenseId}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        DbContext.ChangeTracker.Clear();
        var updated = await DbContext.Expenses.SingleAsync(e => e.Id == expenseId);
        updated.TotalAmount.Should().Be(120.00m);
        updated.Merchant.Should().Be("Updated Store");
    }

    [Fact]
    public async Task Update_Should_ReturnNotFound_When_ExpenseDoesNotExist()
    {
        var userId = Guid.NewGuid().ToString();
        AuthenticateAs(userId, ExpenseUserPermissions);

        var updateRequest = new UpdateExpenseRequest(
            TotalAmount: 50.00m,
            Date: DateTime.UtcNow,
            Merchant: null,
            InvoiceNumber: null,
            InvoiceImageUrl: null,
            Items: new List<RequestExpenseItemData>
            {
                new(Guid.NewGuid(), 50.00m, null)
            });

        var response = await PutAsync($"/api/v1/expenses/{Guid.NewGuid()}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_Should_ReturnNotFound_When_ExpenseBelongsToAnotherUser()
    {
        var ownerId = Guid.NewGuid().ToString();
        AuthenticateAs(ownerId, ExpenseUserPermissions);
        var expenseId = await CreateTestExpense(ownerId);

        RemoveAuthentication();
        var otherId = Guid.NewGuid().ToString();
        AuthenticateAs(otherId, ExpenseUserPermissions);

        var updateRequest = new UpdateExpenseRequest(
            TotalAmount: 50.00m,
            Date: DateTime.UtcNow,
            Merchant: "Hijack",
            InvoiceNumber: null,
            InvoiceImageUrl: null,
            Items: new List<RequestExpenseItemData>
            {
                new(Guid.NewGuid(), 50.00m, null)
            });

        var response = await PutAsync($"/api/v1/expenses/{expenseId}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ═══════════════════════════════════════════════════════════════
    // DELETE
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public async Task Delete_Should_RemoveExpense()
    {
        var userId = Guid.NewGuid().ToString();
        AuthenticateAs(userId, ExpenseUserPermissions);
        var expenseId = await CreateTestExpense(userId);

        var response = await DeleteAsync($"/api/v1/expenses/{expenseId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        DbContext.ChangeTracker.Clear();
        var deleted = await FindAsync<Expense>(expenseId);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task Delete_Should_ReturnNotFound_When_ExpenseDoesNotExist()
    {
        var userId = Guid.NewGuid().ToString();
        AuthenticateAs(userId, ExpenseUserPermissions);

        var response = await DeleteAsync($"/api/v1/expenses/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_Should_ReturnNotFound_When_ExpenseBelongsToAnotherUser()
    {
        var ownerId = Guid.NewGuid().ToString();
        AuthenticateAs(ownerId, ExpenseUserPermissions);
        var expenseId = await CreateTestExpense(ownerId);

        RemoveAuthentication();
        var otherId = Guid.NewGuid().ToString();
        AuthenticateAs(otherId, ExpenseUserPermissions);

        var response = await DeleteAsync($"/api/v1/expenses/{expenseId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        DbContext.ChangeTracker.Clear();
        var stillExists = await FindAsync<Expense>(expenseId);
        stillExists.Should().NotBeNull();
    }

    // ═══════════════════════════════════════════════════════════════
    // HELPERS
    // ═══════════════════════════════════════════════════════════════

    private async Task<Guid> CreateTestExpense(string userId)
    {
        var request = new CreateExpenseRequest(
            Merchant: "Integration Store",
            TotalAmount: 75.00m,
            Date: new DateTime(2025, 6, 15),
            InvoiceNumber: null,
            InvoiceImageUrl: null,
            Items: new List<RequestExpenseItemData>
            {
                new(Guid.NewGuid(), 75.00m, "Test item")
            });

        var response = await PostAsync("/api/v1/expenses", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        return await response.Content.ReadFromJsonAsync<Guid>();
    }
}
