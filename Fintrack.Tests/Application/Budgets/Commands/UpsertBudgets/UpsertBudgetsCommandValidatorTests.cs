using Fintrack.Server.Application.Budgets.Commands.UpsertBudgets;
using Fintrack.Tests.Abstractions;
using FluentValidation.TestHelper;

namespace Fintrack.Tests.Application.Budgets.Commands.UpsertBudgets;

public sealed class UpsertBudgetsCommandValidatorTests : BaseUnitTest
{
    private readonly UpsertBudgetsCommandValidator _validator = new();

    [Fact]
    public void Validate_Should_HaveError_When_UserIdEmpty()
    {
        var command = new UpsertBudgetsCommand(
            UserId: string.Empty,
            Month: 3,
            Year: 2024,
            Budgets: new List<BudgetEntryDto> { new(Guid.NewGuid(), 10m) });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Validate_Should_HaveError_When_MonthOutOfRange()
    {
        var command = new UpsertBudgetsCommand("u1", Month: 13, Year: 2024, Budgets: new List<BudgetEntryDto> { new(Guid.NewGuid(), 10m) });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Month);
    }

    [Fact]
    public void Validate_Should_HaveError_When_CategoryIdIsEmpty()
    {
        var command = new UpsertBudgetsCommand("u1", 3, 2024, new List<BudgetEntryDto> { new(Guid.Empty, 10m) });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("Budgets[0].CategoryId");
    }

    [Fact]
    public void Validate_Should_HaveError_When_AmountNegative()
    {
        var command = new UpsertBudgetsCommand("u1", 3, 2024, new List<BudgetEntryDto> { new(Guid.NewGuid(), -0.01m) });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("Budgets[0].Amount");
    }

    [Fact]
    public void Validate_Should_BeValid_When_AllRulesSatisfied()
    {
        var command = new UpsertBudgetsCommand(
            "u1",
            3,
            2024,
            new List<BudgetEntryDto> { new(Guid.NewGuid(), 0m, IsRecurrent: true) });

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
