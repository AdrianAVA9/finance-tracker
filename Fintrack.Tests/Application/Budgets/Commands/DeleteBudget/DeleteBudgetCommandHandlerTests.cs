using Fintrack.Server.Application.Budgets.Commands.DeleteBudget;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Budgets;
using Fintrack.Tests.Abstractions;
using Fintrack.Tests.TestData.Budgets;
using FluentAssertions;
using NSubstitute;

namespace Fintrack.Tests.Application.Budgets.Commands.DeleteBudget;

public sealed class DeleteBudgetCommandHandlerTests : BaseUnitTest
{
    private readonly IBudgetRepository _budgetRepository = Mock<IBudgetRepository>();
    private readonly IUnitOfWork _unitOfWork = Mock<IUnitOfWork>();

    private DeleteBudgetCommandHandler CreateHandler() =>
        new(_budgetRepository, _unitOfWork);

    [Fact]
    public async Task Handle_Should_RemoveBudget_When_ExistsForUser()
    {
        // Arrange
        var userId = BudgetTestDoubles.DefaultUserId;
        var budget = BudgetTestDoubles.CreateBudget(userId, categoryId: Guid.NewGuid(), month: 1, year: 2024);
        var budgetId = budget.Id;

        _budgetRepository
            .GetByIdAsync(budgetId, userId, CancellationToken)
            .Returns(budget);
        _unitOfWork.SaveChangesAsync(CancellationToken).Returns(1);

        // Act
        var result = await CreateHandler().Handle(new DeleteBudgetCommand(budgetId, userId), CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _budgetRepository.Received(1).Remove(budget);
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken);
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFound_When_BudgetOwnedByAnotherUser()
    {
        // Arrange
        var budget = BudgetTestDoubles.CreateBudget("owner", categoryId: Guid.NewGuid(), month: 1, year: 2024);
        var attackerId = "attacker";

        _budgetRepository
            .GetByIdAsync(budget.Id, attackerId, CancellationToken)
            .Returns((Budget?)null);

        // Act
        var result = await CreateHandler().Handle(new DeleteBudgetCommand(budget.Id, attackerId), CancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BudgetErrors.NotFound);
        _budgetRepository.DidNotReceive().Remove(Arg.Any<Budget>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFound_When_BudgetDoesNotExist()
    {
        // Arrange
        _budgetRepository
            .GetByIdAsync(Arg.Any<Guid>(), BudgetTestDoubles.DefaultUserId, CancellationToken)
            .Returns((Budget?)null);

        // Act
        var result = await CreateHandler().Handle(
            new DeleteBudgetCommand(Guid.NewGuid(), BudgetTestDoubles.DefaultUserId),
            CancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BudgetErrors.NotFound);
        _budgetRepository.DidNotReceive().Remove(Arg.Any<Budget>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
