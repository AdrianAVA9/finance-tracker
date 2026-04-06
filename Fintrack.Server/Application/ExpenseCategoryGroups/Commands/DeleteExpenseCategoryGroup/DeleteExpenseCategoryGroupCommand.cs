using Fintrack.Server.Application.Abstractions.Messaging;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.ExpenseCategories;

namespace Fintrack.Server.Application.ExpenseCategoryGroups.Commands.DeleteExpenseCategoryGroup;

public record DeleteExpenseCategoryGroupCommand(int Id, string UserId) : ICommand;

internal sealed class DeleteExpenseCategoryGroupCommandHandler : ICommandHandler<DeleteExpenseCategoryGroupCommand>
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

    public async Task<Result> Handle(
        DeleteExpenseCategoryGroupCommand request,
        CancellationToken cancellationToken)
    {
        var group = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (group is null)
        {
            return Result.Failure(ExpenseCategoryGroupErrors.NotFound);
        }

        if (group.UserId != null && group.UserId != request.UserId)
        {
            return Result.Failure(ExpenseCategoryGroupErrors.AccessDenied);
        }

        if (!group.IsEditable)
        {
            return Result.Failure(ExpenseCategoryGroupErrors.NotEditable);
        }

        _repository.Remove(group);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
