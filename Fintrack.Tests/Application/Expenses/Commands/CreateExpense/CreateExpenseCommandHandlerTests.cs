using Fintrack.Server.Application.Expenses.Commands.CreateExpense;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Expenses;
using Fintrack.Tests.Abstractions;
using FluentAssertions;
using NSubstitute;

namespace Fintrack.Tests.Application.Expenses.Commands.CreateExpense;

public sealed class CreateExpenseCommandHandlerTests : BaseUnitTest
{
    private readonly IExpenseRepository _expenseRepository = Mock<IExpenseRepository>();
    private readonly IUnitOfWork _unitOfWork = Mock<IUnitOfWork>();

    private CreateExpenseCommandHandler CreateHandler() =>
        new(_expenseRepository, _unitOfWork);

    [Fact]
    public async Task Handle_Should_CreateExpenseAndReturnId_When_Valid()
    {
        // Arrange
        _unitOfWork.SaveChangesAsync(CancellationToken).Returns(1);

        var command = new CreateExpenseCommand(
            UserId: "user-1",
            TotalAmount: 100m,
            Date: DateTime.UtcNow,
            Merchant: "Store A",
            InvoiceNumber: null,
            InvoiceImageUrl: null,
            Items: new List<ExpenseItemDto>
            {
                new(Guid.NewGuid(), 60m, "Item 1"),
                new(Guid.NewGuid(), 40m, "Item 2")
            },
            Invoice: null);

        // Act
        var result = await CreateHandler().Handle(command, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        _expenseRepository.Received(1).Add(Arg.Is<Expense>(e =>
            e.UserId == "user-1" &&
            e.TotalAmount == 100m &&
            e.Items.Count == 2));
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserIdEmpty()
    {
        // Arrange
        var command = new CreateExpenseCommand(
            UserId: "",
            TotalAmount: 100m,
            Date: DateTime.UtcNow,
            Merchant: "Store",
            InvoiceNumber: null,
            InvoiceImageUrl: null,
            Items: new List<ExpenseItemDto>
            {
                new(Guid.NewGuid(), 100m, "Item")
            },
            Invoice: null);

        // Act
        var result = await CreateHandler().Handle(command, CancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ExpenseErrors.UserIdRequired);
        _expenseRepository.DidNotReceive().Add(Arg.Any<Expense>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_TotalAmountInvalid()
    {
        // Arrange
        var command = new CreateExpenseCommand(
            UserId: "user-1",
            TotalAmount: 0m,
            Date: DateTime.UtcNow,
            Merchant: "Store",
            InvoiceNumber: null,
            InvoiceImageUrl: null,
            Items: new List<ExpenseItemDto>
            {
                new(Guid.NewGuid(), 0m, "Item")
            },
            Invoice: null);

        // Act
        var result = await CreateHandler().Handle(command, CancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ExpenseErrors.InvalidTotalAmount);
    }
}
