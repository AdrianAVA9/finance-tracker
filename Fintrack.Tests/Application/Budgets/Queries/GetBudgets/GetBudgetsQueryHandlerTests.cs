using Fintrack.Server.Application.Budgets.Queries.GetBudgets;
using Fintrack.Server.Domain.Budgets;
using Fintrack.Server.Domain.ExpenseCategories;
using Fintrack.Server.Domain.Expenses;
using Fintrack.Server.Domain.Incomes;
using Fintrack.Server.Infrastructure.Data;
using Fintrack.Server.Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace Fintrack.Tests.Application.Budgets.Queries.GetBudgets;

public class GetBudgetsQueryHandlerTests
{
    private ApplicationDbContext GetInMemoryContext()
    {
        var publisher = Substitute.For<IPublisher>();
        publisher.Publish(Arg.Any<INotification>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options, publisher);
    }

    [Fact]
    public async Task Should_Return_Budgets_With_Correct_SpentAmounts()
    {
        using var context = GetInMemoryContext();

        var userId = "test-user";
        var month = 3;
        var year = 2024;

        var group = new ExpenseCategoryGroup { Id = 1, Name = "Vivienda" };
        var category = new ExpenseCategory
        {
            Id = 1,
            Name = "Alquiler",
            GroupId = 1,
            Group = group,
            Color = "#FF0000",
            Icon = "home"
        };
        context.ExpenseCategoryGroups.Add(group);
        context.ExpenseCategories.Add(category);

        var budgetResult = Budget.Create(userId, category.Id, 1000.00m, false, month, year);
        Assert.True(budgetResult.IsSuccess);
        var budget = budgetResult.Value;
        context.Budgets.Add(budget);

        var recurringIncome = new RecurringIncome
        {
            UserId = userId,
            Source = "Salario",
            Amount = 5000.00m,
            CategoryId = 1,
            IsActive = true,
            StartDate = new DateTime(2024, 1, 1),
            NextProcessingDate = new DateTime(2024, 3, 1),
            Frequency = Fintrack.Server.Domain.Enums.RecurringFrequency.Monthly
        };
        context.RecurringIncomes.Add(recurringIncome);

        var expense = new Expense
        {
            Id = 1,
            UserId = userId,
            Date = new DateTime(year, month, 15),
            Merchant = "Rent Corp",
            TotalAmount = 800.00m
        };
        context.Expenses.Add(expense);

        var expenseItem = new ExpenseItem
        {
            Id = 1,
            ExpenseId = 1,
            CategoryId = 1,
            ItemAmount = 800.00m,
            Description = "Marzo Rent"
        };
        context.ExpenseItems.Add(expenseItem);

        await context.SaveChangesAsync();

        var handler = new GetBudgetsQueryHandler(
            new BudgetRepository(context),
            new RecurringIncomeRepository(context),
            new ExpenseRepository(context),
            context);
        var query = new GetBudgetsQuery(userId, month, year);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value.Budgets);
        var dto = result.Value.Budgets.First();
        Assert.Equal(budget.Id, dto.Id);
        Assert.Equal(1, dto.CategoryId);
        Assert.Equal(1000.00m, dto.LimitAmount);
        Assert.Equal(800.00m, dto.SpentAmount);
        Assert.Equal(5000.00m, result.Value.MonthlyIncome);
    }

    [Fact]
    public async Task Should_Return_Empty_List_When_No_Budgets_Exist()
    {
        using var context = GetInMemoryContext();
        var handler = new GetBudgetsQueryHandler(
            new BudgetRepository(context),
            new RecurringIncomeRepository(context),
            new ExpenseRepository(context),
            context);
        var query = new GetBudgetsQuery("user-1", 1, 2024);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value.Budgets);
        Assert.Equal(0, result.Value.MonthlyIncome);
    }
}
