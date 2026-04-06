using Fintrack.Server.Application.Budgets.Commands.CopyPreviousMonthBudgets;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Budgets;
using NSubstitute;
using Xunit;

namespace Fintrack.Tests.Application.Budgets.Commands.CopyPreviousMonthBudgets;

public class CopyPreviousMonthBudgetsCommandHandlerTests
{
    [Fact]
    public async Task Should_Return_AlreadyExists_When_Target_Slot_Exists_But_Was_Not_In_Loaded_Set()
    {
        var userId = "user-1";
        var source = Budget.Create(userId, 7, 200m, true, 2, 2024).Value;

        var repository = Substitute.For<IBudgetRepository>();
        repository.GetUserBudgetsByMonthAsync(userId, 2, 2024, Arg.Any<CancellationToken>())
            .Returns(new List<Budget> { source });
        repository.GetUserBudgetsByMonthAsync(userId, 3, 2024, Arg.Any<CancellationToken>())
            .Returns(new List<Budget>());
        repository.ExistsAsync(userId, 7, 3, 2024, Arg.Any<CancellationToken>())
            .Returns(true);

        var unitOfWork = Substitute.For<IUnitOfWork>();

        var handler = new CopyPreviousMonthBudgetsCommandHandler(repository, unitOfWork);
        var command = new CopyPreviousMonthBudgetsCommand(userId, 3, 2024);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(BudgetErrors.AlreadyExists, result.Error);
        repository.DidNotReceive().Add(Arg.Any<Budget>());
        await unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
