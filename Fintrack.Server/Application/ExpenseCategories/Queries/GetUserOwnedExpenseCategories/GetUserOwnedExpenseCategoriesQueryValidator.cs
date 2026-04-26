using FluentValidation;

namespace Fintrack.Server.Application.ExpenseCategories.Queries.GetUserOwnedExpenseCategories;

internal sealed class GetUserOwnedExpenseCategoriesQueryValidator
    : AbstractValidator<GetUserOwnedExpenseCategoriesQuery>
{
    public GetUserOwnedExpenseCategoriesQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");
    }
}
