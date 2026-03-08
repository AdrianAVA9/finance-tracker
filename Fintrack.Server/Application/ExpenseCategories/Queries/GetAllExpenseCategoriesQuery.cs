using Fintrack.Server.Models;
using MediatR;
using Fintrack.Server.Domain.ExpenseCategories;

namespace Fintrack.Server.Application.ExpenseCategories.Queries
{
    public record GetAllExpenseCategoriesQuery(string UserId) : IRequest<IReadOnlyList<ExpenseCategory>>;

    internal sealed class GetAllExpenseCategoriesQueryHandler : IRequestHandler<GetAllExpenseCategoriesQuery, IReadOnlyList<ExpenseCategory>>
    {
        private readonly IExpenseCategoryRepository _expenseCategoryRepository;

        public GetAllExpenseCategoriesQueryHandler(IExpenseCategoryRepository expenseCategoryRepository)
        {
            _expenseCategoryRepository = expenseCategoryRepository;
        }

        public async Task<IReadOnlyList<ExpenseCategory>> Handle(GetAllExpenseCategoriesQuery request, CancellationToken cancellationToken)
        {
            return await _expenseCategoryRepository.GetAllByUserIdAsync(request.UserId, cancellationToken);
        }
    }
}
