using System;
using FluentValidation;

namespace Fintrack.Server.Application.IncomeCategories.Queries.GetIncomeCategoryById;

internal sealed class GetIncomeCategoryByIdQueryValidator : AbstractValidator<GetIncomeCategoryByIdQuery>
{
    public GetIncomeCategoryByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty)
            .WithMessage("Income category ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");
    }
}
