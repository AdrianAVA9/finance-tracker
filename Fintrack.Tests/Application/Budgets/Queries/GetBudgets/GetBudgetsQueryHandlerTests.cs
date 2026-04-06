using Fintrack.Server.Application.Budgets.Queries.GetBudgets;
using Fintrack.Server.Domain.Budgets;
using Fintrack.Server.Domain.Expenses;
using Fintrack.Server.Domain.Incomes;
using Fintrack.Tests.Abstractions;
using Fintrack.Tests.TestData.Budgets;
using FluentAssertions;
using NSubstitute;

namespace Fintrack.Tests.Application.Budgets.Queries.GetBudgets;

public sealed class GetBudgetsQueryHandlerTests : BaseUnitTest
{
    private readonly IBudgetRepository _budgetRepository = Mock<IBudgetRepository>();
    private readonly IRecurringIncomeRepository _recurringIncomeRepository = Mock<IRecurringIncomeRepository>();
    private readonly IExpenseRepository _expenseRepository = Mock<IExpenseRepository>();
    private readonly GetBudgetsQueryHandler _handler;

    public GetBudgetsQueryHandlerTests()
    {
        _handler = new GetBudgetsQueryHandler(
            _budgetRepository,
            _recurringIncomeRepository,
            _expenseRepository);
    }

    [Fact]
    public async Task Handle_Should_ReturnBudgets_WithSpentAmountsAndIncome_When_DataExists()
    {
        // Arrange
        var userId = BudgetTestDoubles.DefaultUserId;
        var month = 3;
        var year = 2024;
        var category = BudgetTestDoubles.CreateCategory();
        var budget = BudgetTestDoubles.CreateBudgetWithCategory(category, userId, amount: 1000m, month: month, year: year);

        _budgetRepository
            .GetUserBudgetsByMonthWithCategoryAsync(userId, month, year, CancellationToken)
            .Returns(new List<Budget> { budget });

        _recurringIncomeRepository
            .SumActiveAmountForUserBeforeAsync(
                userId,
                Arg.Is<DateTime>(d => d == new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(1)),
                CancellationToken)
            .Returns(5000m);

        var spentByCategory = new Dictionary<int, decimal> { { category.Id, 800m } };
        _expenseRepository
            .SumItemAmountsByCategoryAsync(
                userId,
                Arg.Is<DateTime>(d => d == new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc)),
                Arg.Is<DateTime>(d => d == new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(1)),
                Arg.Is<IReadOnlyCollection<int>>(ids => ids.SequenceEqual(new[] { category.Id })),
                CancellationToken)
            .Returns(spentByCategory);

        var query = new GetBudgetsQuery(userId, month, year);

        // Act
        var result = await _handler.Handle(query, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.MonthlyIncome.Should().Be(5000m);
        result.Value.Budgets.Should().ContainSingle();
        var dto = result.Value.Budgets[0];
        dto.Id.Should().Be(budget.Id);
        dto.CategoryId.Should().Be(category.Id);
        dto.CategoryName.Should().Be(category.Name);
        dto.CategoryIcon.Should().Be(category.Icon);
        dto.CategoryColor.Should().Be(category.Color);
        dto.CategoryGroup.Should().Be(category.Group!.Name);
        dto.LimitAmount.Should().Be(1000m);
        dto.SpentAmount.Should().Be(800m);
        dto.IsRecurrent.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_Should_ReturnEmptyBudgets_When_UserHasNoBudgetsForMonth()
    {
        // Arrange
        _budgetRepository
            .GetUserBudgetsByMonthWithCategoryAsync(BudgetTestDoubles.DefaultUserId, 1, 2024, CancellationToken)
            .Returns(Array.Empty<Budget>());

        _recurringIncomeRepository
            .SumActiveAmountForUserBeforeAsync(
                Arg.Any<string>(),
                Arg.Any<DateTime>(),
                CancellationToken)
            .Returns(0m);

        _expenseRepository
            .SumItemAmountsByCategoryAsync(
                Arg.Any<string>(),
                Arg.Any<DateTime>(),
                Arg.Any<DateTime>(),
                Arg.Is<IReadOnlyCollection<int>>(ids => ids.Count == 0),
                CancellationToken)
            .Returns(new Dictionary<int, decimal>());

        var query = new GetBudgetsQuery(BudgetTestDoubles.DefaultUserId, 1, 2024);

        // Act
        var result = await _handler.Handle(query, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Budgets.Should().BeEmpty();
        result.Value.MonthlyIncome.Should().Be(0m);
    }

    [Fact]
    public async Task Handle_Should_NotReturnOtherMonthBudgets_When_QueryTargetsDifferentMonth()
    {
        // Arrange — repository has no rows for March (e.g. only February exists in DB); read path does not roll forward.
        var userId = BudgetTestDoubles.DefaultUserId;

        _budgetRepository
            .GetUserBudgetsByMonthWithCategoryAsync(userId, 3, 2024, CancellationToken)
            .Returns(Array.Empty<Budget>());

        _recurringIncomeRepository
            .SumActiveAmountForUserBeforeAsync(userId, Arg.Any<DateTime>(), CancellationToken)
            .Returns(0m);

        _expenseRepository
            .SumItemAmountsByCategoryAsync(
                userId,
                Arg.Any<DateTime>(),
                Arg.Any<DateTime>(),
                Arg.Is<IReadOnlyCollection<int>>(ids => ids.Count == 0),
                CancellationToken)
            .Returns(new Dictionary<int, decimal>());

        // Act
        var result = await _handler.Handle(new GetBudgetsQuery(userId, 3, 2024), CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Budgets.Should().BeEmpty();
    }
}
