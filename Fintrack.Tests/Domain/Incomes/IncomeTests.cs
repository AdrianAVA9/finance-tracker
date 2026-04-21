using Fintrack.Server.Domain.Incomes;
using Fintrack.Server.Domain.Incomes.Events;
using FluentAssertions;

namespace Fintrack.Tests.Domain.Incomes;

public sealed class IncomeTests
{
    [Fact]
    public void Create_Should_Succeed_When_ParametersValid()
    {
        var categoryId = Guid.NewGuid();
        var result = Income.Create(
            "user-1",
            "Salary",
            100m,
            categoryId,
            DateTime.UtcNow.Date,
            notes: null);

        result.IsSuccess.Should().BeTrue();
        result.Value.UserId.Should().Be("user-1");
        result.Value.Source.Should().Be("Salary");
        result.Value.Amount.Should().Be(100m);
        result.Value.CategoryId.Should().Be(categoryId);
    }

    [Fact]
    public void Create_Should_RaiseIncomeCreatedDomainEvent_When_Success()
    {
        var result = Income.Create(
            "user-1",
            "Bonus",
            50m,
            Guid.NewGuid(),
            DateTime.UtcNow.Date,
            null);

        result.IsSuccess.Should().BeTrue();
        var evt = result.Value.GetDomainEvents().Should().ContainSingle().Which;
        evt.Should().BeOfType<IncomeCreatedDomainEvent>()
            .Which.IncomeId.Should().Be(result.Value.Id);
    }

    [Fact]
    public void Create_Should_Fail_When_AmountNotPositive()
    {
        var result = Income.Create(
            "user-1",
            "X",
            0m,
            Guid.NewGuid(),
            DateTime.UtcNow.Date,
            null);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(IncomeErrors.InvalidAmount);
    }

    [Fact]
    public void Create_Should_Fail_When_DateTooFarInFuture()
    {
        var far = DateTime.UtcNow.Date.AddYears(2);
        var result = Income.Create(
            "user-1",
            "X",
            10m,
            Guid.NewGuid(),
            far,
            null);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(IncomeErrors.DateTooFarInFuture);
    }

    [Fact]
    public void Update_Should_Succeed_And_RaiseEvent_When_Valid()
    {
        var income = Income.Create(
            "user-1",
            "Old",
            100m,
            Guid.NewGuid(),
            DateTime.UtcNow.Date,
            null).Value;
        income.ClearDomainEvents();

        var update = income.Update(
            "New",
            200m,
            Guid.NewGuid(),
            DateTime.UtcNow.Date,
            "n");

        update.IsSuccess.Should().BeTrue();
        income.Source.Should().Be("New");
        income.Amount.Should().Be(200m);
        income.Notes.Should().Be("n");
        income.GetDomainEvents().Should().ContainSingle()
            .Which.Should().BeOfType<IncomeUpdatedDomainEvent>()
            .Which.IncomeId.Should().Be(income.Id);
    }

    [Fact]
    public void RegisterDeletion_Should_RaiseIncomeDeletedDomainEvent()
    {
        var income = Income.Create(
            "user-1",
            "S",
            1m,
            Guid.NewGuid(),
            DateTime.UtcNow.Date,
            null).Value;
        income.ClearDomainEvents();

        income.RegisterDeletion();

        income.GetDomainEvents().Should().ContainSingle()
            .Which.Should().BeOfType<IncomeDeletedDomainEvent>()
            .Which.IncomeId.Should().Be(income.Id);
    }
}
