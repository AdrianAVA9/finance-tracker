using FluentValidation;

namespace Fintrack.Server.Application.IncomeCategories.Queries.GetIncomeCategories;

internal sealed class GetIncomeCategoriesQueryValidator : AbstractValidator<GetIncomeCategoriesQuery>
{
    public GetIncomeCategoriesQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");
    }
}
