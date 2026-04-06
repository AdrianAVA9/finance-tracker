using Fintrack.Server.Application.Budgets.Commands.UpsertBudgets;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Budgets;
using NSubstitute;
using Xunit;

namespace Fintrack.Tests.Application.Budgets.Commands.UpsertBudgets;

public class UpsertBudgetsCommandHandlerTests
{
    [Fact]
    public async Task Should_Return_AlreadyExists_When_Budget_Exists_But_Was_Not_In_Loaded_Set()
    {
        var userId = "user-1";
        var repository = Substitute.For<IBudgetRepository>();
        repository.GetUserBudgetsByMonthAsync(userId, 3, 2024, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<Budget>());
        repository.ExistsAsync(userId, 5, 3, 2024, Arg.Any<CancellationToken>())
            .Returns(true);

        var unitOfWork = Substitute.For<IUnitOfWork>();

        var handler = new UpsertBudgetsCommandHandler(repository, unitOfWork);
        var command = new UpsertBudgetsCommand(
            userId,
            3,
            2024,
            new List<BudgetEntryDto> { new(5, 100m) });

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(BudgetErrors.AlreadyExists, result.Error);
        repository.DidNotReceive().Add(Arg.Any<Budget>());
        await unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Add_New_Budget_When_Not_Exists()
    {
        var userId = "user-1";
        var repository = Substitute.For<IBudgetRepository>();
        repository.GetUserBudgetsByMonthAsync(userId, 3, 2024, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<Budget>());
        repository.ExistsAsync(userId, 5, 3, 2024, Arg.Any<CancellationToken>())
            .Returns(false);

        var unitOfWork = Substitute.For<IUnitOfWork>();
        unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        var handler = new UpsertBudgetsCommandHandler(repository, unitOfWork);
        var command = new UpsertBudgetsCommand(
            userId,
            3,
            2024,
            new List<BudgetEntryDto> { new(5, 100m) });

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        repository.Received(1).Add(Arg.Any<Budget>());
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
