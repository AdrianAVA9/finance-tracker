using Fintrack.Server.Application.Expenses.Queries.GetExpenseById;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Expenses;
using Fintrack.Tests.Abstractions;
using Fintrack.Tests.TestData.Expenses;
using FluentAssertions;
using NSubstitute;

namespace Fintrack.Tests.Application.Expenses.Queries.GetExpenseById;

public sealed class GetExpenseByIdQueryHandlerTests : BaseUnitTest
{
    private readonly IExpenseRepository _expenseRepository = Mock<IExpenseRepository>();

    private GetExpenseByIdQueryHandler CreateHandler() =>
        new(_expenseRepository);

    [Fact]
    public async Task Handle_Should_ReturnExpenseDetails_When_Found()
    {
        // Arrange
        var expense = ExpenseTestDoubles.CreateExpenseWithItems();
        var userId = ExpenseTestDoubles.DefaultUserId;

        _expenseRepository
            .GetByIdWithFullDetailsAsync(expense.Id, userId, CancellationToken)
            .Returns(expense);

        var query = new GetExpenseByIdQuery(expense.Id, userId);

        // Act
        var result = await CreateHandler().Handle(query, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(expense.Id);
        result.Value.TotalAmount.Should().Be(expense.TotalAmount);
        result.Value.Items.Should().HaveCount(expense.Items.Count);
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFound_When_ExpenseDoesNotExist()
    {
        // Arrange
        _expenseRepository
            .GetByIdWithFullDetailsAsync(Arg.Any<Guid>(), Arg.Any<string>(), CancellationToken)
            .Returns((Expense?)null);

        var query = new GetExpenseByIdQuery(Guid.NewGuid(), "user-1");

        // Act
        var result = await CreateHandler().Handle(query, CancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ExpenseErrors.NotFound);
    }
}
