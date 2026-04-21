using System;
using FluentValidation;

namespace Fintrack.Server.Application.ExpenseCategories.Queries.GetExpenseCategoryById;

internal sealed class GetExpenseCategoryByIdQueryValidator : AbstractValidator<GetExpenseCategoryByIdQuery>
{
    public GetExpenseCategoryByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty)
            .WithMessage("Expense category ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");
    }
}
