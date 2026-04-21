using System;
using FluentValidation;

namespace Fintrack.Server.Application.ExpenseCategories.Commands.DeleteExpenseCategory;

internal sealed class DeleteExpenseCategoryCommandValidator : AbstractValidator<DeleteExpenseCategoryCommand>
{
    public DeleteExpenseCategoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty)
            .WithMessage("Expense category ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");
    }
}
