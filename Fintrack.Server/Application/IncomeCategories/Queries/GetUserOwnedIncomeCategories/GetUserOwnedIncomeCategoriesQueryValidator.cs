using FluentValidation;

namespace Fintrack.Server.Application.IncomeCategories.Queries.GetUserOwnedIncomeCategories;

internal sealed class GetUserOwnedIncomeCategoriesQueryValidator
    : AbstractValidator<GetUserOwnedIncomeCategoriesQuery>
{
    public GetUserOwnedIncomeCategoriesQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");
    }
}
