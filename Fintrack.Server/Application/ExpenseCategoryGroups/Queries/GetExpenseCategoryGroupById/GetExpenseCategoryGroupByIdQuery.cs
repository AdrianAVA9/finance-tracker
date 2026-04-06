using Fintrack.Server.Application.Abstractions.Messaging;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.ExpenseCategories;

namespace Fintrack.Server.Application.ExpenseCategoryGroups.Queries.GetExpenseCategoryGroupById;

public record GetExpenseCategoryGroupByIdQuery(int Id, string UserId) : IQuery<ExpenseCategoryGroup>;

internal sealed class GetExpenseCategoryGroupByIdQueryHandler
    : IQueryHandler<GetExpenseCategoryGroupByIdQuery, ExpenseCategoryGroup>
{
    private readonly IExpenseCategoryGroupRepository _repository;

    public GetExpenseCategoryGroupByIdQueryHandler(IExpenseCategoryGroupRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ExpenseCategoryGroup>> Handle(
        GetExpenseCategoryGroupByIdQuery request,
        CancellationToken cancellationToken)
    {
        var group = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (group is null)
        {
            return Result.Failure<ExpenseCategoryGroup>(ExpenseCategoryGroupErrors.NotFound);
        }

        if (group.UserId != null && group.UserId != request.UserId)
        {
            return Result.Failure<ExpenseCategoryGroup>(ExpenseCategoryGroupErrors.AccessDenied);
        }

        return group;
    }
}
