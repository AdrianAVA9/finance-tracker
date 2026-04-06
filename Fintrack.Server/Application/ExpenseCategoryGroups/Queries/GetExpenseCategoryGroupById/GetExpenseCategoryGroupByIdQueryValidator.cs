using FluentValidation;

namespace Fintrack.Server.Application.ExpenseCategoryGroups.Queries.GetExpenseCategoryGroupById;

internal sealed class GetExpenseCategoryGroupByIdQueryValidator : AbstractValidator<GetExpenseCategoryGroupByIdQuery>
{
    public GetExpenseCategoryGroupByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Expense category group ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");
    }
}
