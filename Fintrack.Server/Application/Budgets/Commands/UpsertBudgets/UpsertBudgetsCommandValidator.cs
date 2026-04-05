using FluentValidation;

namespace Fintrack.Server.Application.Budgets.Commands.UpsertBudgets;

internal sealed class UpsertBudgetsCommandValidator : AbstractValidator<UpsertBudgetsCommand>
{
    public UpsertBudgetsCommandValidator()
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

        RuleFor(x => x.Budgets)
            .NotNull()
            .WithMessage("Budgets list cannot be null");

        RuleForEach(x => x.Budgets).ChildRules(budget =>
        {
            budget.RuleFor(x => x.CategoryId)
                .GreaterThan(0)
                .WithMessage("Category ID must be greater than zero");

            budget.RuleFor(x => x.Amount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Amount must be greater than or equal to zero");
        });
    }
}
