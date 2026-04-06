using FluentValidation;

namespace Fintrack.Server.Application.ExpenseCategoryGroups.Commands.DeleteExpenseCategoryGroup;

internal sealed class DeleteExpenseCategoryGroupCommandValidator : AbstractValidator<DeleteExpenseCategoryGroupCommand>
{
    public DeleteExpenseCategoryGroupCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Expense category group ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");
    }
}
