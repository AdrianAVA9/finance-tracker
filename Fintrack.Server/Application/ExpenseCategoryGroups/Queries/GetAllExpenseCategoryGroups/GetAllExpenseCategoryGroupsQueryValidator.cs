using FluentValidation;

namespace Fintrack.Server.Application.ExpenseCategoryGroups.Queries.GetAllExpenseCategoryGroups;

internal sealed class GetAllExpenseCategoryGroupsQueryValidator : AbstractValidator<GetAllExpenseCategoryGroupsQuery>
{
    public GetAllExpenseCategoryGroupsQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");
    }
}
