using System.Net;
using System.Net.Http.Json;
using Fintrack.IntegrationTests.Infrastructure;
using Fintrack.Server.Application.Budgets.Commands;
using Fintrack.Server.Controllers.Budgets;
using Fintrack.Server.Models;
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

        var budget = new Budget
        {
            UserId = userId,
            CategoryId = category.Id,
            Amount = 500,
            Month = 3,
            Year = 2024
        };
        await AddAsync(budget);

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

        var budget = new Budget
        {
            UserId = userId,
            CategoryId = category.Id,
            Amount = 500,
            Month = 3,
            Year = 2024
        };
        await AddAsync(budget);

        // Act
        var response = await DeleteAsync($"/api/v1/budgets/{budget.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        DbContext.ChangeTracker.Clear();
        var deletedBudget = await FindAsync<Budget>(budget.Id);
        deletedBudget.Should().BeNull();
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
