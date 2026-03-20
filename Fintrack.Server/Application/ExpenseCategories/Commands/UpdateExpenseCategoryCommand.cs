using MediatR;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Exceptions;
using Fintrack.Server.Domain.ExpenseCategories;

namespace Fintrack.Server.Application.ExpenseCategories.Commands
{
    public record UpdateExpenseCategoryCommand(int Id, string Name, string? Description, string? Icon, string? Color, int? GroupId, string UserId) : IRequest;

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

            if (category.UserId != null && category.UserId != request.UserId)
                throw new UnauthorizedAccessException("You do not have permission to update this category.");

            if (!category.IsEditable)
                throw new DomainException("This category cannot be updated.");

            category.Name = request.Name;
            category.Description = request.Description;
            category.Icon = request.Icon;
            category.Color = request.Color;
            category.GroupId = request.GroupId;

            _expenseCategoryRepository.Update(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
