using Fintrack.Server.Application.Expenses.Commands.DeleteExpense;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Expenses;
using Fintrack.Server.Domain.Invoices;
using Fintrack.Tests.Abstractions;
using Fintrack.Tests.TestData.Expenses;
using FluentAssertions;
using NSubstitute;

namespace Fintrack.Tests.Application.Expenses.Commands.DeleteExpense;

public sealed class DeleteExpenseCommandHandlerTests : BaseUnitTest
{
    private readonly IExpenseRepository _expenseRepository = Mock<IExpenseRepository>();
    private readonly IInvoiceRepository _invoiceRepository = Mock<IInvoiceRepository>();
    private readonly IUnitOfWork _unitOfWork = Mock<IUnitOfWork>();

    private DeleteExpenseCommandHandler CreateHandler() =>
        new(_expenseRepository, _invoiceRepository, _unitOfWork);

    [Fact]
    public async Task Handle_Should_RemoveExpense_When_ExistsForUser()
    {
        // Arrange
        var expense = ExpenseTestDoubles.CreateExpenseWithItems();
        var expenseId = expense.Id;
        var userId = ExpenseTestDoubles.DefaultUserId;

        _expenseRepository
            .GetByIdWithFullDetailsAsync(expenseId, userId, CancellationToken)
            .Returns(expense);
        _unitOfWork.SaveChangesAsync(CancellationToken).Returns(1);

        // Act
        var result = await CreateHandler().Handle(new DeleteExpenseCommand(expenseId, userId), CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _expenseRepository.Received(1).Remove(expense);
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken);
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFound_When_ExpenseDoesNotExist()
    {
        // Arrange
        _expenseRepository
            .GetByIdWithFullDetailsAsync(Arg.Any<Guid>(), Arg.Any<string>(), CancellationToken)
            .Returns((Expense?)null);

        // Act
        var result = await CreateHandler().Handle(
            new DeleteExpenseCommand(Guid.NewGuid(), ExpenseTestDoubles.DefaultUserId),
            CancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ExpenseErrors.NotFound);
        _expenseRepository.DidNotReceive().Remove(Arg.Any<Expense>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFound_When_ExpenseOwnedByAnotherUser()
    {
        // Arrange
        var expense = ExpenseTestDoubles.CreateExpense("owner");
        var attackerId = "attacker";

        _expenseRepository
            .GetByIdWithFullDetailsAsync(expense.Id, attackerId, CancellationToken)
            .Returns((Expense?)null);

        // Act
        var result = await CreateHandler().Handle(new DeleteExpenseCommand(expense.Id, attackerId), CancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ExpenseErrors.NotFound);
        _expenseRepository.DidNotReceive().Remove(Arg.Any<Expense>());
    }
}
