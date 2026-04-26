using System;
using FluentValidation;

namespace Fintrack.Server.Application.IncomeCategories.Commands.UpdateIncomeCategory;

internal sealed class UpdateIncomeCategoryCommandValidator : AbstractValidator<UpdateIncomeCategoryCommand>
{
    private const int MaxNameLength = 200;

    public UpdateIncomeCategoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty)
            .WithMessage("Income category ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(MaxNameLength)
            .WithMessage($"Name cannot exceed {MaxNameLength} characters");

        RuleFor(x => x.Icon)
            .MaximumLength(100)
            .When(x => x.Icon is not null);

        RuleFor(x => x.Color)
            .MaximumLength(32)
            .When(x => x.Color is not null);
    }
}
