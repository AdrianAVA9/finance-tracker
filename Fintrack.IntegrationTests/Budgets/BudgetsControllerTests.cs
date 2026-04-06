using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Fintrack.IntegrationTests.Infrastructure;
using Fintrack.Server.Application.Budgets.Commands.UpsertBudgets;
using Fintrack.Server.Application.Budgets.Queries.GetBudgetDetails;
using Fintrack.Server.Api.Controllers.Budgets;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Budgets;
using Fintrack.Server.Domain.Enums;
using Fintrack.Server.Domain.Exceptions;
using Fintrack.Server.Domain.ExpenseCategories;
using Fintrack.Server.Domain.Expenses;
using Fintrack.Server.Domain.Incomes;
using Fintrack.Server.Domain.Invoices;
using Fintrack.Server.Domain.SavingsGoals;
using Fintrack.Server.Domain.Users;
using Fintrack.Server.Infrastructure.Authorization;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Xunit;

namespace Fintrack.IntegrationTests.Budgets;

public class BudgetsControllerTests : BaseIntegrationTest
{
    private static readonly string[] BudgetUserPermissions =
    {
        Permissions.BudgetsRead,
        Permissions.BudgetsWrite
    };

    public BudgetsControllerTests(IntegrationTestWebAppFactory factory) 
        : base(factory)
    {
    }

    [Fact]
    public async Task Get_Should_ReturnBudgets_When_Authenticated()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        AuthenticateAs(userId, BudgetUserPermissions);

        var group = new ExpenseCategoryGroup { Name = "Test group" };
        await AddAsync(group);
        var category = ExpenseCategoryTestHelpers.CreateWithGroup(group, name: "Test category");
        await AddAsync(category);

        var budgetResult = Budget.Create(userId, category.Id, 500, false, 3, 2024);
        budgetResult.IsSuccess.Should().BeTrue();
        await AddAsync(budgetResult.Value);

        // Act
        var response = await GetAsync($"/api/v1/budgets?month=3&year=2024");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var payload = await response.Content.ReadFromJsonAsync<BudgetListResponse>();
        payload.Should().NotBeNull();
        payload!.Budgets.Should().HaveCount(1);
    }

    [Fact]
    public async Task Delete_Should_RemoveBudget()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        AuthenticateAs(userId, BudgetUserPermissions);

        var group = new ExpenseCategoryGroup { Name = "Test group" };
        await AddAsync(group);
        var category = ExpenseCategoryTestHelpers.CreateWithGroup(group, name: "Test category");
        await AddAsync(category);

        var budgetResult = Budget.Create(userId, category.Id, 500, false, 3, 2024);
        budgetResult.IsSuccess.Should().BeTrue();
        var budget = budgetResult.Value;
        await AddAsync(budget);

        // Act
        var response = await DeleteAsync($"/api/v1/budgets/{budget.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        DbContext.ChangeTracker.Clear();
        var deletedBudget = await FindAsync<Budget>(budget.Id);
        deletedBudget.Should().BeNull();
    }

    [Fact]
    public async Task Delete_Should_ReturnNotFound_When_Budget_Does_Not_Exist()
    {
        var userId = Guid.NewGuid().ToString();
        AuthenticateAs(userId, BudgetUserPermissions);

        var missingId = Guid.NewGuid();

        var response = await DeleteAsync($"/api/v1/budgets/{missingId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task BatchUpsert_Should_CreateNewBudgets()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        AuthenticateAs(userId, BudgetUserPermissions);

        var group = new ExpenseCategoryGroup { Name = "Test group" };
        await AddAsync(group);
        var category = ExpenseCategoryTestHelpers.CreateWithGroup(group, name: "Test category");
        await AddAsync(category);

        var request = new UpsertBudgetsRequest(
            Month: 3,
            Year: 2024,
            Budgets: new List<BudgetEntryDto>
            {
                new BudgetEntryDto(category.Id, 1200.50m)
            }
        );

        var response = await PostAsync("/api/v1/budgets/batch", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        DbContext.ChangeTracker.Clear();
        var budgets = await DbContext.Budgets
            .Where(b => b.UserId == userId && b.Month == 3 && b.Year == 2024)
            .ToListAsync();
        
        budgets.Should().HaveCount(1);
        budgets[0].Amount.Should().Be(1200.50m);
    }

    [Fact]
    public async Task CopyPrevious_Should_CreateMarchBudgets_FromFebruary()
    {
        var userId = Guid.NewGuid().ToString();
        AuthenticateAs(userId, BudgetUserPermissions);

        var group = new ExpenseCategoryGroup { Name = "Copy group" };
        await AddAsync(group);
        var category = ExpenseCategoryTestHelpers.CreateWithGroup(group, name: "Copy category");
        await AddAsync(category);

        var feb = Budget.Create(userId, category.Id, 750m, isRecurrent: true, month: 2, year: 2024);
        feb.IsSuccess.Should().BeTrue();
        await AddAsync(feb.Value);

        var response = await PostAsync(
            "/api/v1/budgets/copy-previous",
            new CopyPreviousRequest(Month: 3, Year: 2024));

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        DbContext.ChangeTracker.Clear();
        var march = await DbContext.Budgets
            .Where(b => b.UserId == userId && b.Month == 3 && b.Year == 2024)
            .SingleAsync();
        march.CategoryId.Should().Be(category.Id);
        march.Amount.Should().Be(750m);
        march.IsRecurrent.Should().BeTrue();
    }

    [Fact]
    public async Task CopyPrevious_Should_Succeed_When_PreviousMonthHasNoBudgets()
    {
        var userId = Guid.NewGuid().ToString();
        AuthenticateAs(userId, BudgetUserPermissions);

        var response = await PostAsync(
            "/api/v1/budgets/copy-previous",
            new CopyPreviousRequest(Month: 3, Year: 2024));

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        DbContext.ChangeTracker.Clear();
        var count = await DbContext.Budgets.CountAsync(b => b.UserId == userId);
        count.Should().Be(0);
    }

    [Fact]
    public async Task GetById_Should_ReturnDetails_When_BudgetExists()
    {
        var userId = Guid.NewGuid().ToString();
        AuthenticateAs(userId, BudgetUserPermissions);

        var group = new ExpenseCategoryGroup { Name = "Details group" };
        await AddAsync(group);
        var category = ExpenseCategoryTestHelpers.CreateWithGroup(group, name: "Details category");
        await AddAsync(category);

        var budgetResult = Budget.Create(userId, category.Id, 400m, false, month: 5, year: 2024);
        budgetResult.IsSuccess.Should().BeTrue();
        var budget = budgetResult.Value;
        await AddAsync(budget);

        var response = await GetAsync($"/api/v1/budgets/{budget.Id}?month=5&year=2024");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await response.Content.ReadFromJsonAsync<BudgetDetailsDto>();
        dto.Should().NotBeNull();
        dto!.Id.Should().Be(budget.Id);
        dto.CategoryName.Should().Be("Details category");
        dto.LimitAmount.Should().Be(400m);
        dto.MonthlyHistory.Should().HaveCount(12);
    }

    [Fact]
    public async Task GetById_Should_ReturnNotFound_When_BudgetMissing()
    {
        var userId = Guid.NewGuid().ToString();
        AuthenticateAs(userId, BudgetUserPermissions);

        var response = await GetAsync($"/api/v1/budgets/{Guid.NewGuid()}?month=1&year=2024");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_Should_ReturnNotFound_When_BudgetBelongsToAnotherUser()
    {
        var ownerId = Guid.NewGuid().ToString();
        var otherId = Guid.NewGuid().ToString();

        var group = new ExpenseCategoryGroup { Name = "Iso group" };
        await AddAsync(group);
        var category = ExpenseCategoryTestHelpers.CreateWithGroup(group, name: "Iso category");
        await AddAsync(category);

        var budgetResult = Budget.Create(ownerId, category.Id, 100m, false, 4, 2024);
        budgetResult.IsSuccess.Should().BeTrue();
        await AddAsync(budgetResult.Value);
        var budgetId = budgetResult.Value.Id;

        AuthenticateAs(otherId, BudgetUserPermissions);

        var response = await GetAsync($"/api/v1/budgets/{budgetId}?month=4&year=2024");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private sealed class BudgetListResponse
    {
        [JsonPropertyName("budgets")]
        public List<JsonElement> Budgets { get; init; } = new();

        [JsonPropertyName("monthlyIncome")]
        public decimal MonthlyIncome { get; init; }
    }
}
