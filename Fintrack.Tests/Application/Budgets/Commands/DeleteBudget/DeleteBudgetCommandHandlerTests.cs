using System;
using Fintrack.Server.Application.Budgets.Commands.DeleteBudget;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Budgets;
using NSubstitute;
using Xunit;

namespace Fintrack.Tests.Application.Budgets.Commands.DeleteBudget;

public class DeleteBudgetCommandHandlerTests
{
    [Fact]
    public async Task Should_Delete_Budget_When_Exists_And_Belongs_To_User()
    {
        var userId = "user-1";
        var budget = Budget.Create(userId, 1, 100m, false, 1, 2024).Value;
        var budgetId = budget.Id;

        var repository = Substitute.For<IBudgetRepository>();
        repository.GetByIdAsync(budgetId, userId, Arg.Any<CancellationToken>())
            .Returns(budget);

        var unitOfWork = Substitute.For<IUnitOfWork>();
        unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        var handler = new DeleteBudgetCommandHandler(repository, unitOfWork);
        var command = new DeleteBudgetCommand(budgetId, userId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        repository.Received(1).Remove(budget);
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Not_Delete_Budget_When_Belongs_To_Different_User()
    {
        var budget = Budget.Create("owner", 1, 100m, false, 1, 2024).Value;
        var attackerId = "attacker";
        var budgetId = budget.Id;

        var repository = Substitute.For<IBudgetRepository>();
        repository.GetByIdAsync(budgetId, attackerId, Arg.Any<CancellationToken>())
            .Returns((Budget?)null);

        var unitOfWork = Substitute.For<IUnitOfWork>();

        var handler = new DeleteBudgetCommandHandler(repository, unitOfWork);
        var command = new DeleteBudgetCommand(budgetId, attackerId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        repository.DidNotReceive().Remove(Arg.Any<Budget>());
        await unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Not_Throw_When_Budget_Does_Not_Exist()
    {
        var repository = Substitute.For<IBudgetRepository>();
        repository.GetByIdAsync(Arg.Any<Guid>(), "user-1", Arg.Any<CancellationToken>())
            .Returns((Budget?)null);

        var unitOfWork = Substitute.For<IUnitOfWork>();

        var handler = new DeleteBudgetCommandHandler(repository, unitOfWork);
        var command = new DeleteBudgetCommand(Guid.NewGuid(), "user-1");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        repository.DidNotReceive().Remove(Arg.Any<Budget>());
        await unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
