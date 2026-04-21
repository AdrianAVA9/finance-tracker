using Fintrack.Server.Application.Budgets;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Budgets;
using Fintrack.Tests.Abstractions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace Fintrack.Tests.Application.Budgets;

public sealed class RecurringBudgetRollForwardServiceTests : BaseUnitTest
{
    private static RecurringBudgetRollForwardService CreateSut(
        IBudgetRepository? repository = null,
        IUnitOfWork? unitOfWork = null,
        ILogger<RecurringBudgetRollForwardService>? logger = null) =>
        new(
            repository ?? Mock<IBudgetRepository>(),
            unitOfWork ?? Mock<IUnitOfWork>(),
            logger ?? NullLogger<RecurringBudgetRollForwardService>.Instance);

    [Fact]
    public async Task ProcessAsync_Should_QueryPreviousMonth_When_CurrentIsMarch()
    {
        var repository = Mock<IBudgetRepository>();
        repository
            .GetRecurrentBudgetsForMonthForAllUsersAsync(2, 2024, CancellationToken)
            .Returns(Array.Empty<Budget>());

        var sut = CreateSut(repository);
        var utc = new DateTime(2024, 3, 10, 12, 0, 0, DateTimeKind.Utc);

        await sut.ProcessAsync(utc, CancellationToken);

        await repository.Received(1).GetRecurrentBudgetsForMonthForAllUsersAsync(
            2,
            2024,
            CancellationToken);
    }

    [Fact]
    public async Task ProcessAsync_Should_NotSave_When_NoRecurringBudgets()
    {
        var repository = Mock<IBudgetRepository>();
        var unitOfWork = Mock<IUnitOfWork>();
        repository
            .GetRecurrentBudgetsForMonthForAllUsersAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(Array.Empty<Budget>());

        var sut = CreateSut(repository, unitOfWork);

        await sut.ProcessAsync(new DateTime(2024, 3, 1, 0, 0, 0, DateTimeKind.Utc), CancellationToken);

        await unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ProcessAsync_Should_AddBudgetAndSave_When_SlotIsFree()
    {
        var userId = "user-a";
        var categoryId = Guid.NewGuid();
        var february = Budget.Create(userId, categoryId, 250m, isRecurrent: true, month: 2, year: 2024);
        february.IsSuccess.Should().BeTrue();

        var repository = Mock<IBudgetRepository>();
        var unitOfWork = Mock<IUnitOfWork>();
        repository
            .GetRecurrentBudgetsForMonthForAllUsersAsync(2, 2024, CancellationToken)
            .Returns(new[] { february.Value });
        repository
            .ExistsAsync(userId, categoryId, 3, 2024, CancellationToken)
            .Returns(false);

        var sut = CreateSut(repository, unitOfWork);

        await sut.ProcessAsync(new DateTime(2024, 3, 5, 8, 0, 0, DateTimeKind.Utc), CancellationToken);

        repository.Received(1).Add(Arg.Is<Budget>(b =>
            b.UserId == userId &&
            b.CategoryId == categoryId &&
            b.Month == 3 &&
            b.Year == 2024 &&
            b.Amount == 250m &&
            b.IsRecurrent));
        await unitOfWork.Received(1).SaveChangesAsync(CancellationToken);
    }

    [Fact]
    public async Task ProcessAsync_Should_NotSave_When_AllSlotsAlreadyExist()
    {
        var userId = "user-b";
        var categoryId = Guid.NewGuid();
        var february = Budget.Create(userId, categoryId, 50m, true, 2, 2024);
        february.IsSuccess.Should().BeTrue();

        var repository = Mock<IBudgetRepository>();
        var unitOfWork = Mock<IUnitOfWork>();
        repository
            .GetRecurrentBudgetsForMonthForAllUsersAsync(2, 2024, CancellationToken)
            .Returns(new[] { february.Value });
        repository
            .ExistsAsync(userId, categoryId, 3, 2024, CancellationToken)
            .Returns(true);

        var sut = CreateSut(repository, unitOfWork);

        await sut.ProcessAsync(new DateTime(2024, 3, 1, 0, 0, 0, DateTimeKind.Utc), CancellationToken);

        repository.DidNotReceive().Add(Arg.Any<Budget>());
        await unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ProcessAsync_Should_SaveOnce_When_OneAdds_And_OneAlreadyExists()
    {
        var cat1 = Guid.NewGuid();
        var cat2 = Guid.NewGuid();
        var u1 = "u1";
        var b1 = Budget.Create(u1, cat1, 10m, true, 2, 2024).Value;
        var b2 = Budget.Create(u1, cat2, 20m, true, 2, 2024).Value;

        var repository = Mock<IBudgetRepository>();
        var unitOfWork = Mock<IUnitOfWork>();
        repository
            .GetRecurrentBudgetsForMonthForAllUsersAsync(2, 2024, CancellationToken)
            .Returns(new[] { b1, b2 });
        repository.ExistsAsync(u1, cat1, 3, 2024, CancellationToken).Returns(false);
        repository.ExistsAsync(u1, cat2, 3, 2024, CancellationToken).Returns(true);

        var sut = CreateSut(repository, unitOfWork);

        await sut.ProcessAsync(new DateTime(2024, 3, 15, 0, 0, 0, DateTimeKind.Utc), CancellationToken);

        repository.Received(1).Add(Arg.Any<Budget>());
        await unitOfWork.Received(1).SaveChangesAsync(CancellationToken);
    }

    [Fact]
    public async Task ProcessAsync_Should_HandleYearRollover_ForJanuary()
    {
        var repository = Mock<IBudgetRepository>();
        repository
            .GetRecurrentBudgetsForMonthForAllUsersAsync(12, 2023, CancellationToken)
            .Returns(Array.Empty<Budget>());

        var sut = CreateSut(repository);
        var utc = new DateTime(2024, 1, 1, 6, 0, 0, DateTimeKind.Utc);

        await sut.ProcessAsync(utc, CancellationToken);

        await repository.Received(1).GetRecurrentBudgetsForMonthForAllUsersAsync(12, 2023, CancellationToken);
    }
}
