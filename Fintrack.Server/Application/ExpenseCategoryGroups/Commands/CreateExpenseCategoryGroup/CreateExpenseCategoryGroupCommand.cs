using Fintrack.Server.Application.Abstractions.Messaging;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.ExpenseCategories;

namespace Fintrack.Server.Application.ExpenseCategoryGroups.Commands.CreateExpenseCategoryGroup;

public record CreateExpenseCategoryGroupCommand(
    string Name,
    string? Description,
    string UserId
) : ICommand<Guid>;

internal sealed class CreateExpenseCategoryGroupCommandHandler : ICommandHandler<CreateExpenseCategoryGroupCommand, Guid>
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

    public async Task<Result<Guid>> Handle(
        CreateExpenseCategoryGroupCommand request,
        CancellationToken cancellationToken)
    {
        var createResult = ExpenseCategoryGroup.CreateForUser(
            request.Name,
            request.Description,
            request.UserId);

        if (createResult.IsFailure)
        {
            return Result.Failure<Guid>(createResult.Error);
        }

        var group = createResult.Value;

        _repository.Add(group);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return group.Id;
    }
}
