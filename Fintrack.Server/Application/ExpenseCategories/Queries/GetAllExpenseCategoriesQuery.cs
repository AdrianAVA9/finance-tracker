using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Budgets;
using Fintrack.Server.Domain.Enums;
using Fintrack.Server.Domain.Exceptions;
using Fintrack.Server.Domain.ExpenseCategories;
using Fintrack.Server.Domain.Expenses;
using Fintrack.Server.Domain.Incomes;
using Fintrack.Server.Domain.Invoices;
using Fintrack.Server.Domain.SavingsGoals;
using Fintrack.Server.Domain.Users;
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
