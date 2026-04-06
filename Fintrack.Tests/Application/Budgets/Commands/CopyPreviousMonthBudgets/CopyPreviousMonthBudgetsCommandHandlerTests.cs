using Fintrack.Server.Application.Budgets.Commands.CopyPreviousMonthBudgets;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Budgets;
using Fintrack.Tests.Abstractions;
using Fintrack.Tests.TestData.Budgets;
using FluentAssertions;
using NSubstitute;

namespace Fintrack.Tests.Application.Budgets.Commands.CopyPreviousMonthBudgets;

public sealed class CopyPreviousMonthBudgetsCommandHandlerTests : BaseUnitTest
{
    private readonly IBudgetRepository _budgetRepository = Mock<IBudgetRepository>();
    private readonly IUnitOfWork _unitOfWork = Mock<IUnitOfWork>();

    private CopyPreviousMonthBudgetsCommandHandler CreateHandler() =>
        new(_budgetRepository, _unitOfWork);

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_TargetSlotExists_But_NotInLoadedSet()
    {
        // Arrange
        var userId = BudgetTestDoubles.DefaultUserId;
        var source = BudgetTestDoubles.CreateBudget(userId, categoryId: 7, amount: 200m, isRecurrent: true, month: 2, year: 2024);

        _budgetRepository
            .GetUserBudgetsByMonthAsync(userId, 2, 2024, CancellationToken)
            .Returns(new List<Budget> { source });
        _budgetRepository
            .GetUserBudgetsByMonthAsync(userId, 3, 2024, CancellationToken)
            .Returns(new List<Budget>());
        _budgetRepository
            .ExistsAsync(userId, 7, 3, 2024, CancellationToken)
            .Returns(true);

        // Act
        var result = await CreateHandler().Handle(new CopyPreviousMonthBudgetsCommand(userId, 3, 2024), CancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BudgetErrors.AlreadyExists);
        _budgetRepository.DidNotReceive().Add(Arg.Any<Budget>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_SucceedWithoutSave_When_SourceMonthIsEmpty()
    {
        // Arrange
        var userId = BudgetTestDoubles.DefaultUserId;
        _budgetRepository
            .GetUserBudgetsByMonthAsync(userId, 2, 2024, CancellationToken)
            .Returns(Array.Empty<Budget>());

        // Act
        var result = await CreateHandler().Handle(new CopyPreviousMonthBudgetsCommand(userId, 3, 2024), CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        await _budgetRepository.DidNotReceive()
            .GetUserBudgetsByMonthAsync(userId, 3, 2024, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_CopyFromDecember_When_TargetIsJanuary()
    {
        // Arrange — previous month of Jan 2025 is Dec 2024
        var userId = BudgetTestDoubles.DefaultUserId;
        var source = BudgetTestDoubles.CreateBudget(userId, categoryId: 3, amount: 150m, month: 12, year: 2024);

        _budgetRepository
            .GetUserBudgetsByMonthAsync(userId, 12, 2024, CancellationToken)
            .Returns(new List<Budget> { source });
        _budgetRepository
            .GetUserBudgetsByMonthAsync(userId, 1, 2025, CancellationToken)
            .Returns(Array.Empty<Budget>());
        _budgetRepository
            .ExistsAsync(userId, 3, 1, 2025, CancellationToken)
            .Returns(false);
        _unitOfWork.SaveChangesAsync(CancellationToken).Returns(1);

        // Act
        var result = await CreateHandler().Handle(new CopyPreviousMonthBudgetsCommand(userId, 1, 2025), CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _budgetRepository.Received(1).Add(Arg.Is<Budget>(b =>
            b.Month == 1 && b.Year == 2025 && b.CategoryId == 3 && b.Amount == 150m));
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken);
    }

    [Fact]
    public async Task Handle_Should_UpdateTarget_When_CategoryAlreadyExistsInTargetMonth()
    {
        // Arrange
        var userId = BudgetTestDoubles.DefaultUserId;
        var source = BudgetTestDoubles.CreateBudget(userId, categoryId: 7, amount: 200m, isRecurrent: true, month: 2, year: 2024);
        var targetExisting = BudgetTestDoubles.CreateBudget(userId, categoryId: 7, amount: 50m, isRecurrent: false, month: 3, year: 2024);

        _budgetRepository
            .GetUserBudgetsByMonthAsync(userId, 2, 2024, CancellationToken)
            .Returns(new List<Budget> { source });
        _budgetRepository
            .GetUserBudgetsByMonthAsync(userId, 3, 2024, CancellationToken)
            .Returns(new List<Budget> { targetExisting });
        _unitOfWork.SaveChangesAsync(CancellationToken).Returns(1);

        // Act
        var result = await CreateHandler().Handle(new CopyPreviousMonthBudgetsCommand(userId, 3, 2024), CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        targetExisting.Amount.Should().Be(200m);
        targetExisting.IsRecurrent.Should().BeTrue();
        _budgetRepository.Received(1).Update(targetExisting);
        _budgetRepository.DidNotReceive().Add(Arg.Any<Budget>());
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken);
    }
}
