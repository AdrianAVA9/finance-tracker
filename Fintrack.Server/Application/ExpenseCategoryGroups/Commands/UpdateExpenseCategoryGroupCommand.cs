using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Exceptions;
using Fintrack.Server.Domain.ExpenseCategories;
using MediatR;

namespace Fintrack.Server.Application.ExpenseCategoryGroups.Commands
{
    public record UpdateExpenseCategoryGroupCommand(
        int Id,
        string Name,
        string? Description,
        string UserId) : IRequest;

    internal sealed class UpdateExpenseCategoryGroupCommandHandler : IRequestHandler<UpdateExpenseCategoryGroupCommand>
    {
        private readonly IExpenseCategoryGroupRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateExpenseCategoryGroupCommandHandler(
            IExpenseCategoryGroupRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateExpenseCategoryGroupCommand request, CancellationToken cancellationToken)
        {
            var group = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (group == null)
                throw new NotFoundException(nameof(Models.ExpenseCategoryGroup), request.Id);

            if (group.UserId != null && group.UserId != request.UserId)
                throw new UnauthorizedAccessException("You do not have permission to update this group.");

            if (!group.IsEditable)
                throw new DomainException("This group cannot be updated.");

            group.Name = request.Name;
            group.Description = request.Description;

            _repository.Update(group);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
