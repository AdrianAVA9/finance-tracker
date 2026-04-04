using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.ExpenseCategories;
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

namespace Fintrack.Server.Application.ExpenseCategoryGroups.Commands
{
    public record CreateExpenseCategoryGroupCommand(
        string Name,
        string? Description,
        string UserId) : IRequest<int>;

    internal sealed class CreateExpenseCategoryGroupCommandHandler : IRequestHandler<CreateExpenseCategoryGroupCommand, int>
    {
        private readonly IExpenseCategoryGroupRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateExpenseCategoryGroupCommandHandler(
            IExpenseCategoryGroupRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(CreateExpenseCategoryGroupCommand request, CancellationToken cancellationToken)
        {
            var group = new ExpenseCategoryGroup
            {
                Name = request.Name,
                Description = request.Description,
                UserId = request.UserId,
                IsEditable = true
            };

            _repository.Add(group);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return group.Id;
        }
    }
}
