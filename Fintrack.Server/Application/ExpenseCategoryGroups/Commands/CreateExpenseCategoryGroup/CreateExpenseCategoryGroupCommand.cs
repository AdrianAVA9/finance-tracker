using Fintrack.Server.Application.Abstractions.Messaging;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.ExpenseCategories;

namespace Fintrack.Server.Application.ExpenseCategoryGroups.Commands.CreateExpenseCategoryGroup;

public record CreateExpenseCategoryGroupCommand(
    string Name,
    string? Description,
    string UserId
) : ICommand<int>;

internal sealed class CreateExpenseCategoryGroupCommandHandler : ICommandHandler<CreateExpenseCategoryGroupCommand, int>
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

    public async Task<Result<int>> Handle(
        CreateExpenseCategoryGroupCommand request,
        CancellationToken cancellationToken)
    {
        var group = new ExpenseCategoryGroup
        {
            Name = request.Name,
            Description = request.Description,
            UserId = request.UserId,
            IsEditable = true
        };

        _repository.Add(group);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return group.Id;
    }
}
