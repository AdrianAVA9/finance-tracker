using Fintrack.Server.Application.Budgets.Queries.GetBudgets;
using Fintrack.Tests.Abstractions;
using FluentValidation.TestHelper;

namespace Fintrack.Tests.Application.Budgets.Queries.GetBudgets;

public sealed class GetBudgetsQueryValidatorTests : BaseUnitTest
{
    private readonly GetBudgetsQueryValidator _validator = new();

    [Fact]
    public void Validate_Should_HaveError_When_UserIdEmpty()
    {
        var query = new GetBudgetsQuery(string.Empty, 1, 2024);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Validate_Should_BeValid_When_AllRulesSatisfied()
    {
        var query = new GetBudgetsQuery("u1", 12, 2024);

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
