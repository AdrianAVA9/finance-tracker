using MediatR;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Exceptions;
using Fintrack.Server.Domain.ExpenseCategories;

namespace Fintrack.Server.Application.ExpenseCategories.Commands
{
    public record UpdateExpenseCategoryCommand(int Id, string Name, string? Icon, string? Color, string UserId) : IRequest;

    internal sealed class UpdateExpenseCategoryCommandHandler : IRequestHandler<UpdateExpenseCategoryCommand>
    {
        private readonly IExpenseCategoryRepository _expenseCategoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateExpenseCategoryCommandHandler(
            IExpenseCategoryRepository expenseCategoryRepository,
            IUnitOfWork unitOfWork)
        {
            _expenseCategoryRepository = expenseCategoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateExpenseCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _expenseCategoryRepository.GetByIdAsync(request.Id, cancellationToken);

            if (category is null)
            {
                throw new NotFoundException(nameof(Models.ExpenseCategory), request.Id);
            }

            if (category.UserId != request.UserId)
            {
                throw new UnauthorizedAccessException("You do not have permission to modify this category.");
            }
            
            if (category.IsSystem)
            {
                 throw new DomainException("Cannot modify a system category.");
            }

            category.Name = request.Name;
            category.Icon = request.Icon;
            category.Color = request.Color;

            _expenseCategoryRepository.Update(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
