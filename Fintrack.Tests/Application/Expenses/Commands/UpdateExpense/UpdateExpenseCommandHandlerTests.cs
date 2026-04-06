using Fintrack.Server.Application.Expenses.Commands.UpdateExpense;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Expenses;
using Fintrack.Tests.Abstractions;
using Fintrack.Tests.TestData.Expenses;
using FluentAssertions;
using NSubstitute;

namespace Fintrack.Tests.Application.Expenses.Commands.UpdateExpense;

public sealed class UpdateExpenseCommandHandlerTests : BaseUnitTest
{
    private readonly IExpenseRepository _expenseRepository = Mock<IExpenseRepository>();
    private readonly IUnitOfWork _unitOfWork = Mock<IUnitOfWork>();

    private UpdateExpenseCommandHandler CreateHandler() =>
        new(_expenseRepository, _unitOfWork);

    [Fact]
    public async Task Handle_Should_UpdateExpense_When_ExistsForUser()
    {
        // Arrange
        var expense = ExpenseTestDoubles.CreateExpenseWithItems();
        var expenseId = expense.Id;
        var userId = ExpenseTestDoubles.DefaultUserId;

        _expenseRepository
            .GetByIdWithItemsAsync(expenseId, userId, CancellationToken)
            .Returns(expense);
        _unitOfWork.SaveChangesAsync(CancellationToken).Returns(1);

        var newCategoryId = Guid.NewGuid();
        var command = new UpdateExpenseCommand(
            expenseId, userId, 200m, DateTime.UtcNow, "Updated Merchant", "INV-001", null,
            new List<UpdateExpenseItemDto>
            {
                new(newCategoryId, 200m, "Updated item")
            });

        // Act
        var result = await CreateHandler().Handle(command, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        expense.TotalAmount.Should().Be(200m);
        expense.Merchant.Should().Be("Updated Merchant");
        expense.Items.Should().ContainSingle();
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken);
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFound_When_ExpenseDoesNotExist()
    {
        // Arrange
        _expenseRepository
            .GetByIdWithItemsAsync(Arg.Any<Guid>(), Arg.Any<string>(), CancellationToken)
            .Returns((Expense?)null);

        var command = new UpdateExpenseCommand(
            Guid.NewGuid(), "user-1", 100m, DateTime.UtcNow, "M", null, null,
            new List<UpdateExpenseItemDto> { new(Guid.NewGuid(), 100m, "Item") });

        // Act
        var result = await CreateHandler().Handle(command, CancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ExpenseErrors.NotFound);
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UpdateAmountInvalid()
    {
        // Arrange
        var expense = ExpenseTestDoubles.CreateExpenseWithItems();

        _expenseRepository
            .GetByIdWithItemsAsync(expense.Id, ExpenseTestDoubles.DefaultUserId, CancellationToken)
            .Returns(expense);

        var command = new UpdateExpenseCommand(
            expense.Id, ExpenseTestDoubles.DefaultUserId, 0m, DateTime.UtcNow, "M", null, null,
            new List<UpdateExpenseItemDto> { new(Guid.NewGuid(), 0m, "Item") });

        // Act
        var result = await CreateHandler().Handle(command, CancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ExpenseErrors.InvalidTotalAmount);
    }
}
