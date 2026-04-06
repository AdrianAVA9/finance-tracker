using Fintrack.Server.Application.Abstractions.Messaging;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.ExpenseCategories;

namespace Fintrack.Server.Application.ExpenseCategories.Commands.UpdateExpenseCategory;

public record UpdateExpenseCategoryCommand(
    Guid Id,
    string Name,
    string? Description,
    string? Icon,
    string? Color,
    int? GroupId,
    string UserId
) : ICommand;

internal sealed class UpdateExpenseCategoryCommandHandler : ICommandHandler<UpdateExpenseCategoryCommand>
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

    public async Task<Result> Handle(UpdateExpenseCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _expenseCategoryRepository.GetByIdAsync(request.Id, cancellationToken);

        if (category is null)
        {
            return Result.Failure(ExpenseCategoryErrors.NotFound);
        }

        if (category.UserId != null && category.UserId != request.UserId)
        {
            return Result.Failure(ExpenseCategoryErrors.AccessDenied);
        }

        var updateResult = category.Update(
            request.Name,
            request.Description,
            request.Icon,
            request.Color,
            request.GroupId);

        if (updateResult.IsFailure)
        {
            return Result.Failure(updateResult.Error);
        }

        _expenseCategoryRepository.Update(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
