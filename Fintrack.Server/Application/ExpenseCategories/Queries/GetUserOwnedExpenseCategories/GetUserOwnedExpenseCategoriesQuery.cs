using Fintrack.Server.Application.Abstractions.Messaging;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.ExpenseCategories;

namespace Fintrack.Server.Application.ExpenseCategories.Queries.GetUserOwnedExpenseCategories;

public record GetUserOwnedExpenseCategoriesQuery(string UserId) : IQuery<IReadOnlyList<ExpenseCategory>>;

internal sealed class GetUserOwnedExpenseCategoriesQueryHandler
    : IQueryHandler<GetUserOwnedExpenseCategoriesQuery, IReadOnlyList<ExpenseCategory>>
{
    private readonly IExpenseCategoryRepository _expenseCategoryRepository;

    public GetUserOwnedExpenseCategoriesQueryHandler(IExpenseCategoryRepository expenseCategoryRepository)
    {
        _expenseCategoryRepository = expenseCategoryRepository;
    }

    public async Task<Result<IReadOnlyList<ExpenseCategory>>> Handle(
        GetUserOwnedExpenseCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var list = await _expenseCategoryRepository.GetUserOwnedByUserIdAsync(
            request.UserId,
            cancellationToken);
        return Result.Success(list);
    }
}
