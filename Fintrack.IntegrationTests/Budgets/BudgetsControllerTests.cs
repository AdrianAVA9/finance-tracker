using System.Net;
using System.Net.Http.Json;
using Fintrack.IntegrationTests.Infrastructure;
using Fintrack.Server.Application.Budgets.Commands.UpsertBudgets;
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
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Xunit;

namespace Fintrack.IntegrationTests.Budgets;

public class BudgetsControllerTests : BaseIntegrationTest
{
    public BudgetsControllerTests(IntegrationTestWebAppFactory factory) 
        : base(factory)
    {
    }

    [Fact]
    public async Task Get_Should_ReturnBudgets_When_Authenticated()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        AuthenticateAs(userId);

        var group = new ExpenseCategoryGroup { Name = "Test group" };
        var category = new ExpenseCategory { Name = "Test category", Group = group };
        await AddAsync(group);
        await AddAsync(category);

        var budgetResult = Budget.Create(userId, category.Id, 500, false, 3, 2024);
        budgetResult.IsSuccess.Should().BeTrue();
        await AddAsync(budgetResult.Value);

        // Act
        var response = await GetAsync($"/api/v1/budgets?month=3&year=2024");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var budgets = await response.Content.ReadFromJsonAsync<List<object>>();
        budgets.Should().NotBeNull();
        budgets.Should().HaveCount(1);
    }

    [Fact]
    public async Task Delete_Should_RemoveBudget()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        AuthenticateAs(userId);

        var group = new ExpenseCategoryGroup { Name = "Test group" };
        var category = new ExpenseCategory { Name = "Test category", Group = group };
        await AddAsync(group);
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
        AuthenticateAs(userId);

        var missingId = Guid.NewGuid();

        var response = await DeleteAsync($"/api/v1/budgets/{missingId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task BatchUpsert_Should_CreateNewBudgets()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        AuthenticateAs(userId);

        var group = new ExpenseCategoryGroup { Name = "Test group" };
        var category = new ExpenseCategory { Name = "Test category", Group = group };
        await AddAsync(group);
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
}
