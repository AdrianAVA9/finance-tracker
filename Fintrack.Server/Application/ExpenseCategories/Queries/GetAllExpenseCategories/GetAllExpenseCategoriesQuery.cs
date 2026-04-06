using Fintrack.Server.Application.Abstractions.Messaging;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.ExpenseCategories;

namespace Fintrack.Server.Application.ExpenseCategories.Queries.GetAllExpenseCategories;

public record GetAllExpenseCategoriesQuery(string UserId) : IQuery<IReadOnlyList<ExpenseCategory>>;

internal sealed class GetAllExpenseCategoriesQueryHandler
    : IQueryHandler<GetAllExpenseCategoriesQuery, IReadOnlyList<ExpenseCategory>>
{
    private readonly IExpenseCategoryRepository _expenseCategoryRepository;

    public GetAllExpenseCategoriesQueryHandler(IExpenseCategoryRepository expenseCategoryRepository)
    {
        _expenseCategoryRepository = expenseCategoryRepository;
    }

    public async Task<Result<IReadOnlyList<ExpenseCategory>>> Handle(
        GetAllExpenseCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var list = await _expenseCategoryRepository.GetAllByUserIdAsync(request.UserId, cancellationToken);
        return Result.Success(list);
    }
}
