using Fintrack.Server.Application.Budgets.Commands.UpsertBudgets;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Budgets;
using Fintrack.Tests.Abstractions;
using Fintrack.Tests.TestData.Budgets;
using FluentAssertions;
using NSubstitute;

namespace Fintrack.Tests.Application.Budgets.Commands.UpsertBudgets;

public sealed class UpsertBudgetsCommandHandlerTests : BaseUnitTest
{
    private readonly IBudgetRepository _budgetRepository = Mock<IBudgetRepository>();
    private readonly IUnitOfWork _unitOfWork = Mock<IUnitOfWork>();

    private UpsertBudgetsCommandHandler CreateHandler() =>
        new(_budgetRepository, _unitOfWork);

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_SlotExists_But_NotInLoadedSet()
    {
        // Arrange (TOCTOU: empty list then ExistsAsync true)
        var userId = BudgetTestDoubles.DefaultUserId;
        _budgetRepository
            .GetUserBudgetsByMonthAsync(userId, 3, 2024, CancellationToken)
            .Returns(Array.Empty<Budget>());
        _budgetRepository
            .ExistsAsync(userId, 5, 3, 2024, CancellationToken)
            .Returns(true);

        var command = new UpsertBudgetsCommand(
            userId,
            3,
            2024,
            new List<BudgetEntryDto> { new(5, 100m) });

        // Act
        var result = await CreateHandler().Handle(command, CancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BudgetErrors.AlreadyExists);
        _budgetRepository.DidNotReceive().Add(Arg.Any<Budget>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_AddBudget_When_SlotIsFree()
    {
        // Arrange
        var userId = BudgetTestDoubles.DefaultUserId;
        _budgetRepository
            .GetUserBudgetsByMonthAsync(userId, 3, 2024, CancellationToken)
            .Returns(Array.Empty<Budget>());
        _budgetRepository
            .ExistsAsync(userId, 5, 3, 2024, CancellationToken)
            .Returns(false);
        _unitOfWork.SaveChangesAsync(CancellationToken).Returns(1);

        var command = new UpsertBudgetsCommand(
            userId,
            3,
            2024,
            new List<BudgetEntryDto> { new(5, 100m, IsRecurrent: true) });

        // Act
        var result = await CreateHandler().Handle(command, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _budgetRepository.Received(1).Add(Arg.Is<Budget>(b =>
            b.UserId == userId &&
            b.CategoryId == 5 &&
            b.Amount == 100m &&
            b.IsRecurrent &&
            b.Month == 3 &&
            b.Year == 2024));
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken);
    }

    [Fact]
    public async Task Handle_Should_UpdateExisting_When_CategoryAlreadyHasBudget()
    {
        // Arrange
        var userId = BudgetTestDoubles.DefaultUserId;
        var existing = BudgetTestDoubles.CreateBudget(userId, categoryId: 5, amount: 50m, month: 3, year: 2024);
        _budgetRepository
            .GetUserBudgetsByMonthAsync(userId, 3, 2024, CancellationToken)
            .Returns(new List<Budget> { existing });
        _unitOfWork.SaveChangesAsync(CancellationToken).Returns(1);

        var command = new UpsertBudgetsCommand(
            userId,
            3,
            2024,
            new List<BudgetEntryDto> { new(5, 250m, IsRecurrent: true) });

        // Act
        var result = await CreateHandler().Handle(command, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        existing.Amount.Should().Be(250m);
        existing.IsRecurrent.Should().BeTrue();
        _budgetRepository.Received(1).Update(existing);
        _budgetRepository.DidNotReceive().Add(Arg.Any<Budget>());
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken);
    }

    [Fact]
    public async Task Handle_Should_UseLastEntry_When_DuplicateCategoryIdsInPayload()
    {
        // Arrange
        var userId = BudgetTestDoubles.DefaultUserId;
        _budgetRepository
            .GetUserBudgetsByMonthAsync(userId, 3, 2024, CancellationToken)
            .Returns(Array.Empty<Budget>());
        _budgetRepository.ExistsAsync(userId, 5, 3, 2024, CancellationToken).Returns(false);
        _unitOfWork.SaveChangesAsync(CancellationToken).Returns(1);

        var command = new UpsertBudgetsCommand(
            userId,
            3,
            2024,
            new List<BudgetEntryDto>
            {
                new(5, 100m),
                new(5, 200m)
            });

        // Act
        var result = await CreateHandler().Handle(command, CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _budgetRepository.Received(1).Add(Arg.Is<Budget>(b => b.CategoryId == 5 && b.Amount == 200m));
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UpdateAmountIsNegative()
    {
        // Arrange — bypass validator scenario: domain rejects negative updates
        var userId = BudgetTestDoubles.DefaultUserId;
        var existing = BudgetTestDoubles.CreateBudget(userId, categoryId: 5, amount: 50m, month: 3, year: 2024);
        _budgetRepository
            .GetUserBudgetsByMonthAsync(userId, 3, 2024, CancellationToken)
            .Returns(new List<Budget> { existing });

        var command = new UpsertBudgetsCommand(
            userId,
            3,
            2024,
            new List<BudgetEntryDto> { new(5, -1m) });

        // Act
        var result = await CreateHandler().Handle(command, CancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BudgetErrors.NegativeAmount);
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
