using Fintrack.Server.Application.Abstractions.Messaging;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.ExpenseCategories;

namespace Fintrack.Server.Application.ExpenseCategories.Queries.GetExpenseCategoryById;

public record GetExpenseCategoryByIdQuery(Guid Id, string UserId) : IQuery<ExpenseCategory>;

internal sealed class GetExpenseCategoryByIdQueryHandler : IQueryHandler<GetExpenseCategoryByIdQuery, ExpenseCategory>
{
    private readonly IExpenseCategoryRepository _expenseCategoryRepository;

    public GetExpenseCategoryByIdQueryHandler(IExpenseCategoryRepository expenseCategoryRepository)
    {
        _expenseCategoryRepository = expenseCategoryRepository;
    }

    public async Task<Result<ExpenseCategory>> Handle(
        GetExpenseCategoryByIdQuery request,
        CancellationToken cancellationToken)
    {
        var category = await _expenseCategoryRepository.GetByIdAsync(request.Id, cancellationToken);

        if (category is null)
        {
            return Result.Failure<ExpenseCategory>(ExpenseCategoryErrors.NotFound);
        }

        if (category.UserId != null && category.UserId != request.UserId)
        {
            return Result.Failure<ExpenseCategory>(ExpenseCategoryErrors.AccessDenied);
        }

        return category;
    }
}
