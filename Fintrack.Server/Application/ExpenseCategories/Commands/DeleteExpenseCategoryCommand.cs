using MediatR;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Exceptions;
using Fintrack.Server.Domain.ExpenseCategories;

namespace Fintrack.Server.Application.ExpenseCategories.Commands
{
    public record DeleteExpenseCategoryCommand(int Id, string UserId) : IRequest;

    internal sealed class DeleteExpenseCategoryCommandHandler : IRequestHandler<DeleteExpenseCategoryCommand>
    {
        private readonly IExpenseCategoryRepository _expenseCategoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteExpenseCategoryCommandHandler(
            IExpenseCategoryRepository expenseCategoryRepository,
            IUnitOfWork unitOfWork)
        {
            _expenseCategoryRepository = expenseCategoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteExpenseCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _expenseCategoryRepository.GetByIdAsync(request.Id, cancellationToken);

            if (category is null)
            {
                throw new NotFoundException(nameof(Models.ExpenseCategory), request.Id);
            }

            if (category.UserId != null && category.UserId != request.UserId)
                throw new UnauthorizedAccessException("You do not have permission to delete this category.");

            if (!category.IsEditable)
                throw new DomainException("This category cannot be deleted.");
            

            _expenseCategoryRepository.Remove(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
