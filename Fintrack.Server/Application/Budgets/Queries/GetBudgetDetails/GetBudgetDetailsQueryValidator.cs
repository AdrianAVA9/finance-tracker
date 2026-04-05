using FluentValidation;

namespace Fintrack.Server.Application.Budgets.Queries.GetBudgetDetails;

internal sealed class GetBudgetDetailsQueryValidator : AbstractValidator<GetBudgetDetailsQuery>
{
    public GetBudgetDetailsQueryValidator()
    {
        RuleFor(x => x.BudgetId)
            .NotEqual(Guid.Empty)
            .WithMessage("Budget ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.Month)
            .InclusiveBetween(1, 12)
            .WithMessage("Month must be between 1 and 12");

        RuleFor(x => x.Year)
            .InclusiveBetween(1900, 2100)
            .WithMessage("Year must be valid");
    }
}
