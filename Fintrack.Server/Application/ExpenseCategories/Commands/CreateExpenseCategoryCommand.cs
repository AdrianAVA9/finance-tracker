using MediatR;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.ExpenseCategories;
using Fintrack.Server.Models;

namespace Fintrack.Server.Application.ExpenseCategories.Commands
{
    public record CreateExpenseCategoryCommand(string Name, string? Icon, string? Color, string UserId) : IRequest<int>;

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
                Icon = request.Icon,
                Color = request.Color,
                UserId = request.UserId,
                IsSystem = false
            };

            _expenseCategoryRepository.Add(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return category.Id;
        }
    }
}
