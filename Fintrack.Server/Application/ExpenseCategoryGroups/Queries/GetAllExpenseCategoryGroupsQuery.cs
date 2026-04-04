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

namespace Fintrack.Server.Application.ExpenseCategoryGroups.Queries
{
    public record GetAllExpenseCategoryGroupsQuery(string UserId) : IRequest<IReadOnlyList<ExpenseCategoryGroup>>;

    internal sealed class GetAllExpenseCategoryGroupsQueryHandler : IRequestHandler<GetAllExpenseCategoryGroupsQuery, IReadOnlyList<ExpenseCategoryGroup>>
    {
        private readonly IExpenseCategoryGroupRepository _repository;

        public GetAllExpenseCategoryGroupsQueryHandler(IExpenseCategoryGroupRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<ExpenseCategoryGroup>> Handle(GetAllExpenseCategoryGroupsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetAllByUserIdAsync(request.UserId, cancellationToken);
        }
    }
}
