using FluentValidation;
using Fintrack.Server.Application.Abstractions.Messaging;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Expenses;

namespace Fintrack.Server.Application.Expenses.Commands.DeleteExpense;

public record DeleteExpenseCommand(Guid Id, string UserId) : ICommand;

internal sealed class DeleteExpenseCommandValidator : AbstractValidator<DeleteExpenseCommand>
{
    public DeleteExpenseCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty)
            .WithMessage("Expense ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");
    }
}

internal sealed class DeleteExpenseCommandHandler : ICommandHandler<DeleteExpenseCommand>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteExpenseCommandHandler(
        IExpenseRepository expenseRepository,
        IUnitOfWork unitOfWork)
    {
        _expenseRepository = expenseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        DeleteExpenseCommand request,
        CancellationToken cancellationToken)
    {
        var expense = await _expenseRepository.GetByIdWithItemsAsync(
            request.Id,
            request.UserId,
            cancellationToken);

        if (expense is null)
        {
            return Result.Failure(ExpenseErrors.NotFound);
        }

        expense.RegisterDeletion();
        _expenseRepository.Remove(expense);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
