using Fintrack.Server.Domain.Budgets;
using Fintrack.Server.Domain.Budgets.Events;
using Xunit;

namespace Fintrack.Tests.Domain.Budgets;

public class BudgetTests
{
    [Fact]
    public void RegisterDeletion_Raises_BudgetDeletedDomainEvent_With_Budget_Id()
    {
        var budget = Budget.Create("user-1", 1, 100m, false, 3, 2024).Value;
        budget.ClearDomainEvents();

        budget.RegisterDeletion();

        var evt = Assert.Single(budget.GetDomainEvents());
        var deleted = Assert.IsType<BudgetDeletedDomainEvent>(evt);
        Assert.Equal(budget.Id, deleted.BudgetId);
    }
}
