using FluentValidation;

namespace Fintrack.Server.Application.ExpenseCategoryGroups.Commands.UpdateExpenseCategoryGroup;

internal sealed class UpdateExpenseCategoryGroupCommandValidator : AbstractValidator<UpdateExpenseCategoryGroupCommand>
{
    public UpdateExpenseCategoryGroupCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Expense category group ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .When(x => x.Description is not null);
    }
}
