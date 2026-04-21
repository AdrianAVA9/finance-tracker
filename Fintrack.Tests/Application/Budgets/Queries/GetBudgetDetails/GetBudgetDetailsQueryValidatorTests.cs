using Fintrack.Server.Application.Budgets.Queries.GetBudgetDetails;
using Fintrack.Tests.Abstractions;
using FluentValidation.TestHelper;

namespace Fintrack.Tests.Application.Budgets.Queries.GetBudgetDetails;

public sealed class GetBudgetDetailsQueryValidatorTests : BaseUnitTest
{
    private readonly GetBudgetDetailsQueryValidator _validator = new();

    [Fact]
    public void Validate_Should_HaveError_When_BudgetIdEmpty()
    {
        var query = new GetBudgetDetailsQuery(Guid.Empty, "u1", 3, 2024);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.BudgetId);
    }

    [Fact]
    public void Validate_Should_BeValid_When_AllRulesSatisfied()
    {
        var query = new GetBudgetDetailsQuery(Guid.NewGuid(), "u1", 3, 2024);

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
