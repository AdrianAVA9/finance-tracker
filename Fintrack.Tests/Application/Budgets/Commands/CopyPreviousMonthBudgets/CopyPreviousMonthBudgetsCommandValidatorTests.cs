using Fintrack.Server.Application.Budgets.Commands.CopyPreviousMonthBudgets;
using Fintrack.Tests.Abstractions;
using FluentValidation.TestHelper;

namespace Fintrack.Tests.Application.Budgets.Commands.CopyPreviousMonthBudgets;

public sealed class CopyPreviousMonthBudgetsCommandValidatorTests : BaseUnitTest
{
    private readonly CopyPreviousMonthBudgetsCommandValidator _validator = new();

    [Fact]
    public void Validate_Should_HaveError_When_UserIdEmpty()
    {
        var command = new CopyPreviousMonthBudgetsCommand(string.Empty, TargetMonth: 3, TargetYear: 2024);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Validate_Should_HaveError_When_TargetMonthInvalid()
    {
        var command = new CopyPreviousMonthBudgetsCommand("u1", TargetMonth: 0, TargetYear: 2024);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.TargetMonth);
    }

    [Fact]
    public void Validate_Should_BeValid_When_AllRulesSatisfied()
    {
        var command = new CopyPreviousMonthBudgetsCommand("u1", TargetMonth: 1, TargetYear: 2025);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
