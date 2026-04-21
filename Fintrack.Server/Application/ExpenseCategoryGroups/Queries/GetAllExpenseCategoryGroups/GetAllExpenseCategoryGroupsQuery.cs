using Fintrack.Server.Application.Abstractions.Messaging;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.ExpenseCategories;

namespace Fintrack.Server.Application.ExpenseCategoryGroups.Queries.GetAllExpenseCategoryGroups;

public record GetAllExpenseCategoryGroupsQuery(string UserId) : IQuery<IReadOnlyList<ExpenseCategoryGroup>>;

internal sealed class GetAllExpenseCategoryGroupsQueryHandler
    : IQueryHandler<GetAllExpenseCategoryGroupsQuery, IReadOnlyList<ExpenseCategoryGroup>>
{
    private readonly IExpenseCategoryGroupRepository _repository;

    public GetAllExpenseCategoryGroupsQueryHandler(IExpenseCategoryGroupRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IReadOnlyList<ExpenseCategoryGroup>>> Handle(
        GetAllExpenseCategoryGroupsQuery request,
        CancellationToken cancellationToken)
    {
        var list = await _repository.GetAllByUserIdAsync(request.UserId, cancellationToken);
        return Result.Success(list);
    }
}
