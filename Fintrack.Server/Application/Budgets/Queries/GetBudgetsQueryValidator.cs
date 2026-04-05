using Fintrack.Server.Application.Budgets.Queries;
using FluentValidation;

namespace Fintrack.Server.Application.Budgets.Queries;

internal sealed class GetBudgetsQueryValidator : AbstractValidator<GetBudgetsQuery>
{
    public GetBudgetsQueryValidator()
    {
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
