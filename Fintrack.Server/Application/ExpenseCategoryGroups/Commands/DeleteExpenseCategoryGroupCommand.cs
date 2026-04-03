using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Exceptions;
using Fintrack.Server.Domain.ExpenseCategories;
using MediatR;

namespace Fintrack.Server.Application.ExpenseCategoryGroups.Commands
{
    public record DeleteExpenseCategoryGroupCommand(int Id, string UserId) : IRequest;

    internal sealed class DeleteExpenseCategoryGroupCommandHandler : IRequestHandler<DeleteExpenseCategoryGroupCommand>
    {
        private readonly IExpenseCategoryGroupRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteExpenseCategoryGroupCommandHandler(
            IExpenseCategoryGroupRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteExpenseCategoryGroupCommand request, CancellationToken cancellationToken)
        {
            var group = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (group == null)
                throw new NotFoundException(nameof(ExpenseCategoryGroup), request.Id);

            if (group.UserId != null && group.UserId != request.UserId)
                throw new UnauthorizedAccessException("You do not have permission to delete this group.");

            if (!group.IsEditable)
                throw new DomainException("This group cannot be deleted.");

            _repository.Remove(group);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
