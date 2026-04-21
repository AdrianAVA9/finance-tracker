using Fintrack.Server.Application.Abstractions.Messaging;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.ExpenseCategories;

namespace Fintrack.Server.Application.ExpenseCategories.Commands.DeleteExpenseCategory;

public record DeleteExpenseCategoryCommand(Guid Id, string UserId) : ICommand;

internal sealed class DeleteExpenseCategoryCommandHandler : ICommandHandler<DeleteExpenseCategoryCommand>
{
    private readonly IExpenseCategoryRepository _expenseCategoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteExpenseCategoryCommandHandler(
        IExpenseCategoryRepository expenseCategoryRepository,
        IUnitOfWork unitOfWork)
    {
        _expenseCategoryRepository = expenseCategoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteExpenseCategoryCommand request, CancellationToken cancellationToken)
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

        var deleteResult = category.RegisterDeletion();
        if (deleteResult.IsFailure)
        {
            return Result.Failure(deleteResult.Error);
        }

        _expenseCategoryRepository.Remove(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
