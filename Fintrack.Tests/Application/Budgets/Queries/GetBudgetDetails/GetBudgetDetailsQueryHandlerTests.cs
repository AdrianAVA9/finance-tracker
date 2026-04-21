using Fintrack.Server.Application.Budgets.Queries.GetBudgetDetails;
using Fintrack.Server.Domain.Budgets;
using Fintrack.Server.Domain.Expenses;
using Fintrack.Tests.Abstractions;
using Fintrack.Tests.TestData.Budgets;
using Fintrack.Tests.TestData.Expenses;
using FluentAssertions;
using NSubstitute;

namespace Fintrack.Tests.Application.Budgets.Queries.GetBudgetDetails;

public sealed class GetBudgetDetailsQueryHandlerTests : BaseUnitTest
{
    private readonly IBudgetRepository _budgetRepository = Mock<IBudgetRepository>();
    private readonly IExpenseRepository _expenseRepository = Mock<IExpenseRepository>();

    private GetBudgetDetailsQueryHandler CreateHandler() =>
        new(_budgetRepository, _expenseRepository);

    [Fact]
    public async Task Handle_Should_ReturnNotFound_When_BudgetMissing()
    {
        // Arrange
        var budgetId = Guid.NewGuid();
        _budgetRepository
            .GetByIdWithCategoryAsync(budgetId, BudgetTestDoubles.DefaultUserId, CancellationToken)
            .Returns((Budget?)null);

        // Act
        var result = await CreateHandler().Handle(
            new GetBudgetDetailsQuery(budgetId, BudgetTestDoubles.DefaultUserId, Month: 3, Year: 2024),
            CancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BudgetErrors.NotFound);
        await _expenseRepository.DidNotReceive()
            .GetItemsForUserCategoryInDateRangeAsync(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnTwelveMonthsOfHistory_When_ExpensesExist()
    {
        // Arrange
        var userId = BudgetTestDoubles.DefaultUserId;
        var category = BudgetTestDoubles.CreateCategory(name: "Food");
        var budget = BudgetTestDoubles.CreateBudgetWithCategory(category, userId, amount: 400m, month: 3, year: 2024);

        _budgetRepository
            .GetByIdWithCategoryAsync(budget.Id, userId, CancellationToken)
            .Returns(budget);

        var expenseDate = new DateTime(2024, 3, 10, 0, 0, 0, DateTimeKind.Utc);
        var parentExpense = ExpenseTestDoubles.CreateExpense(userId, 25m, expenseDate, "Cafe");

        var items = new List<ExpenseItem>
        {
            ExpenseTestDoubles.CreateItemWithExpense(category.Id, 25m, "Lunch", parentExpense)
        };

        _expenseRepository
            .GetItemsForUserCategoryInDateRangeAsync(
                userId,
                category.Id,
                Arg.Any<DateTime>(),
                Arg.Any<DateTime>(),
                CancellationToken)
            .Returns(items);

        // Act
        var result = await CreateHandler().Handle(
            new GetBudgetDetailsQuery(budget.Id, userId, Month: 3, Year: 2024),
            CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(budget.Id);
        result.Value.CategoryName.Should().Be("Food");
        result.Value.LimitAmount.Should().Be(400m);
        result.Value.MonthlyHistory.Should().HaveCount(12);

        var march = result.Value.MonthlyHistory.Single(m => m.Month == 3 && m.Year == 2024);
        march.TotalExpense.Should().Be(25m);
        march.Expenses.Should().ContainSingle(e => e.Description == "Lunch" && e.Amount == 25m);
    }

    [Fact]
    public async Task Handle_Should_UseNoDescription_When_ItemDescriptionNull()
    {
        // Arrange
        var userId = BudgetTestDoubles.DefaultUserId;
        var category = BudgetTestDoubles.CreateCategory();
        var budget = BudgetTestDoubles.CreateBudgetWithCategory(category, userId, month: 2, year: 2024);

        _budgetRepository
            .GetByIdWithCategoryAsync(budget.Id, userId, CancellationToken)
            .Returns(budget);

        var expenseDate = new DateTime(2024, 2, 5, 0, 0, 0, DateTimeKind.Utc);
        var parentExpense = ExpenseTestDoubles.CreateExpense(userId, 10m, expenseDate, "X");

        var items = new List<ExpenseItem>
        {
            ExpenseTestDoubles.CreateItemWithExpense(category.Id, 10m, null, parentExpense)
        };

        _expenseRepository
            .GetItemsForUserCategoryInDateRangeAsync(
                userId,
                category.Id,
                Arg.Any<DateTime>(),
                Arg.Any<DateTime>(),
                CancellationToken)
            .Returns(items);

        // Act
        var result = await CreateHandler().Handle(
            new GetBudgetDetailsQuery(budget.Id, userId, Month: 2, Year: 2024),
            CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var feb = result.Value.MonthlyHistory.Single(m => m.Month == 2 && m.Year == 2024);
        feb.Expenses.Should().ContainSingle(e => e.Description == "No description");
    }
}
