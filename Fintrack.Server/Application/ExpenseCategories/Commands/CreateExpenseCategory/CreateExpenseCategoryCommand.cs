using Fintrack.Server.Application.Abstractions.Messaging;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.ExpenseCategories;

namespace Fintrack.Server.Application.ExpenseCategories.Commands.CreateExpenseCategory;

public record CreateExpenseCategoryCommand(
    string Name,
    string? Description,
    string? Icon,
    string? Color,
    int? GroupId,
    string UserId
) : ICommand<Guid>;

internal sealed class CreateExpenseCategoryCommandHandler : ICommandHandler<CreateExpenseCategoryCommand, Guid>
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

    public async Task<Result<Guid>> Handle(CreateExpenseCategoryCommand request, CancellationToken cancellationToken)
    {
        var createResult = ExpenseCategory.Create(
            request.Name,
            request.Description,
            request.Icon,
            request.Color,
            request.GroupId,
            request.UserId,
            isEditable: true);

        if (createResult.IsFailure)
        {
            return Result.Failure<Guid>(createResult.Error);
        }

        var category = createResult.Value;

        _expenseCategoryRepository.Add(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return category.Id;
    }
}
