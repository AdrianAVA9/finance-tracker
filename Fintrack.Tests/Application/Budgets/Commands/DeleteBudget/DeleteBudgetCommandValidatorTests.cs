using Fintrack.Server.Application.Budgets.Commands.DeleteBudget;
using Fintrack.Tests.Abstractions;
using FluentValidation.TestHelper;

namespace Fintrack.Tests.Application.Budgets.Commands.DeleteBudget;

public sealed class DeleteBudgetCommandValidatorTests : BaseUnitTest
{
    private readonly DeleteBudgetCommandValidator _validator = new();

    [Fact]
    public void Validate_Should_HaveError_When_IdEmpty()
    {
        var command = new DeleteBudgetCommand(Guid.Empty, "user-1");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validate_Should_HaveError_When_UserIdEmpty()
    {
        var command = new DeleteBudgetCommand(Guid.NewGuid(), string.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Validate_Should_BeValid_When_AllRulesSatisfied()
    {
        var command = new DeleteBudgetCommand(Guid.NewGuid(), "user-1");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
