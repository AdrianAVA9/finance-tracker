using FluentValidation;

namespace Fintrack.Server.Application.ExpenseCategoryGroups.Commands.CreateExpenseCategoryGroup;

internal sealed class CreateExpenseCategoryGroupCommandValidator : AbstractValidator<CreateExpenseCategoryGroupCommand>
{
    public CreateExpenseCategoryGroupCommandValidator()
    {
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
