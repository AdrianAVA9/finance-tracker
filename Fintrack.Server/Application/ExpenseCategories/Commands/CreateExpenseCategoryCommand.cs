using MediatR;
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

namespace Fintrack.Server.Application.ExpenseCategories.Commands
{
    public record CreateExpenseCategoryCommand(string Name, string? Description, string? Icon, string? Color, int? GroupId, string UserId) : IRequest<int>;

    internal sealed class CreateExpenseCategoryCommandHandler : IRequestHandler<CreateExpenseCategoryCommand, int>
    {
        private readonly IExpenseCategoryRepository _expenseCategoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateExpenseCategoryCommandHandler(
            IExpenseCategoryRepository expenseCategoryRepository,
            IUnitOfWork unitOfWork)
        {
            _expenseCategoryRepository = expenseCategoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(CreateExpenseCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = new ExpenseCategory
            {
                Name = request.Name,
                Description = request.Description,
                Icon = request.Icon,
                Color = request.Color,
                GroupId = request.GroupId,
                UserId = request.UserId,
                IsEditable = true
            };

            _expenseCategoryRepository.Add(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return category.Id;
        }
    }
}
