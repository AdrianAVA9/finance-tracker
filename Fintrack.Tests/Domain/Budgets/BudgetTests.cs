using Fintrack.Server.Domain.Budgets;
using Fintrack.Server.Domain.Budgets.Events;
using FluentAssertions;

namespace Fintrack.Tests.Domain.Budgets;

public sealed class BudgetTests
{
    [Fact]
    public void Create_Should_Succeed_When_ParametersValid()
    {
        var result = Budget.Create("user-1", categoryId: 3, amount: 100m, isRecurrent: false, month: 6, year: 2025);

        result.IsSuccess.Should().BeTrue();
        result.Value.UserId.Should().Be("user-1");
        result.Value.CategoryId.Should().Be(3);
        result.Value.Amount.Should().Be(100m);
        result.Value.Month.Should().Be(6);
        result.Value.Year.Should().Be(2025);
    }

    [Fact]
    public void Create_Should_RaiseBudgetCreatedDomainEvent_When_Success()
    {
        var result = Budget.Create("user-1", 1, 50m, false, 1, 2024);

        result.IsSuccess.Should().BeTrue();
        var evt = result.Value.GetDomainEvents().Should().ContainSingle().Which;
        evt.Should().BeOfType<BudgetCreatedDomainEvent>()
            .Which.BudgetId.Should().Be(result.Value.Id);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-0.01)]
    public void Create_Should_Fail_When_AmountNegative(decimal amount)
    {
        var result = Budget.Create("user-1", 1, amount, false, 1, 2024);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BudgetErrors.NegativeAmount);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(13)]
    public void Create_Should_Fail_When_MonthInvalid(int month)
    {
        var result = Budget.Create("user-1", 1, 10m, false, month, 2024);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BudgetErrors.InvalidMonth);
    }

    [Theory]
    [InlineData(1899)]
    [InlineData(2101)]
    public void Create_Should_Fail_When_YearInvalid(int year)
    {
        var result = Budget.Create("user-1", 1, 10m, false, 1, year);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BudgetErrors.InvalidYear);
    }

    [Fact]
    public void Update_Should_Succeed_And_RaiseEvent_When_Valid()
    {
        var budget = Budget.Create("user-1", 1, 100m, false, 1, 2024).Value;
        budget.ClearDomainEvents();

        var update = budget.Update(200m, isRecurrent: true);

        update.IsSuccess.Should().BeTrue();
        budget.Amount.Should().Be(200m);
        budget.IsRecurrent.Should().BeTrue();
        budget.GetDomainEvents().Should().ContainSingle()
            .Which.Should().BeOfType<BudgetUpdatedDomainEvent>()
            .Which.BudgetId.Should().Be(budget.Id);
    }

    [Fact]
    public void Update_Should_Fail_When_AmountNegative()
    {
        var budget = Budget.Create("user-1", 1, 100m, false, 1, 2024).Value;
        budget.ClearDomainEvents();

        var update = budget.Update(-1m, false);

        update.IsFailure.Should().BeTrue();
        update.Error.Should().Be(BudgetErrors.NegativeAmount);
        budget.Amount.Should().Be(100m);
        budget.GetDomainEvents().Should().BeEmpty();
    }

    [Fact]
    public void RegisterDeletion_Should_Raise_BudgetDeletedDomainEvent_With_Budget_Id()
    {
        var budget = Budget.Create("user-1", 1, 100m, false, 3, 2024).Value;
        budget.ClearDomainEvents();

        budget.RegisterDeletion();

        var evt = budget.GetDomainEvents().Should().ContainSingle().Which;
        var deleted = evt.Should().BeOfType<BudgetDeletedDomainEvent>().Subject;
        deleted.BudgetId.Should().Be(budget.Id);
    }
}
