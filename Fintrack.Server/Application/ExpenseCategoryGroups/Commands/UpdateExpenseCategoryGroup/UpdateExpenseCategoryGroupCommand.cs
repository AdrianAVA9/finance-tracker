using Fintrack.Server.Application.Abstractions.Messaging;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.ExpenseCategories;

namespace Fintrack.Server.Application.ExpenseCategoryGroups.Commands.UpdateExpenseCategoryGroup;

public record UpdateExpenseCategoryGroupCommand(
    int Id,
    string Name,
    string? Description,
    string UserId
) : ICommand;

internal sealed class UpdateExpenseCategoryGroupCommandHandler : ICommandHandler<UpdateExpenseCategoryGroupCommand>
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

    public async Task<Result> Handle(
        UpdateExpenseCategoryGroupCommand request,
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

        group.Name = request.Name;
        group.Description = request.Description;

        _repository.Update(group);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
