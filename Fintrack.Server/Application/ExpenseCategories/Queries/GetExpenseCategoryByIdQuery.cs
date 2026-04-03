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
using Fintrack.Server.Domain.Exceptions;
using Fintrack.Server.Domain.ExpenseCategories;

namespace Fintrack.Server.Application.ExpenseCategories.Queries
{
    public record GetExpenseCategoryByIdQuery(int Id, string UserId) : IRequest<ExpenseCategory>;

    internal sealed class GetExpenseCategoryByIdQueryHandler : IRequestHandler<GetExpenseCategoryByIdQuery, ExpenseCategory>
    {
        private readonly IExpenseCategoryRepository _expenseCategoryRepository;

        public GetExpenseCategoryByIdQueryHandler(IExpenseCategoryRepository expenseCategoryRepository)
        {
            _expenseCategoryRepository = expenseCategoryRepository;
        }

        public async Task<ExpenseCategory> Handle(GetExpenseCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var category = await _expenseCategoryRepository.GetByIdAsync(request.Id, cancellationToken);

            if (category is null)
            {
                throw new NotFoundException(nameof(ExpenseCategory), request.Id);
            }

             if (category.UserId != null && category.UserId != request.UserId)
            {
                throw new UnauthorizedAccessException("You do not have permission to view this category.");
            }

            return category;
        }
    }
}
