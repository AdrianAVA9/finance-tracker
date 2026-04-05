using FluentValidation;

namespace Fintrack.Server.Application.Budgets.Commands.CopyPreviousMonthBudgets;

internal sealed class CopyPreviousMonthBudgetsCommandValidator : AbstractValidator<CopyPreviousMonthBudgetsCommand>
{
    public CopyPreviousMonthBudgetsCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.TargetMonth)
            .InclusiveBetween(1, 12)
            .WithMessage("Target Month must be between 1 and 12");

        RuleFor(x => x.TargetYear)
            .InclusiveBetween(1900, 2100)
            .WithMessage("Target Year must be valid");
    }
}
