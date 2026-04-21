using FluentValidation;

namespace Fintrack.Server.Application.ExpenseCategories.Queries.GetAllExpenseCategories;

internal sealed class GetAllExpenseCategoriesQueryValidator : AbstractValidator<GetAllExpenseCategoriesQuery>
{
    public GetAllExpenseCategoriesQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");
    }
}
