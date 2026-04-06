using FluentValidation;
using Fintrack.Server.Application.Abstractions.Messaging;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Expenses;

namespace Fintrack.Server.Application.Expenses.Commands.UpdateExpense;

public record UpdateExpenseItemDto(Guid CategoryId, decimal ItemAmount, string? Description);

public record UpdateExpenseCommand(
    Guid Id,
    string UserId,
    decimal TotalAmount,
    DateTime Date,
    string? Merchant,
    string? InvoiceNumber,
    string? InvoiceImageUrl,
    List<UpdateExpenseItemDto> Items
) : ICommand;

internal sealed class UpdateExpenseCommandValidator : AbstractValidator<UpdateExpenseCommand>
{
    public UpdateExpenseCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty)
            .WithMessage("Expense ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.TotalAmount)
            .GreaterThan(0)
            .WithMessage("Total amount must be greater than zero");

        RuleFor(x => x.Date)
            .NotEmpty()
            .WithMessage("Date is required");

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Expense must contain at least one item");

        RuleFor(x => x)
            .Must(cmd => cmd.Items != null
                          && cmd.Items.Count > 0
                          && cmd.Items.Sum(i => i.ItemAmount) == cmd.TotalAmount)
            .WithName("MathematicalIntegrity")
            .WithMessage("The sum of item amounts must equal the total amount");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.CategoryId)
                .NotEmpty()
                .WithMessage("Category ID is required for each item");

            item.RuleFor(i => i.ItemAmount)
                .GreaterThan(0)
                .WithMessage("Item amount must be greater than zero");
        });
    }
}

internal sealed class UpdateExpenseCommandHandler : ICommandHandler<UpdateExpenseCommand>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateExpenseCommandHandler(
        IExpenseRepository expenseRepository,
        IUnitOfWork unitOfWork)
    {
        _expenseRepository = expenseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        UpdateExpenseCommand request,
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

        var updateResult = expense.Update(
            request.TotalAmount,
            request.Date,
            request.Merchant,
            request.InvoiceNumber,
            request.InvoiceImageUrl);

        if (updateResult.IsFailure)
        {
            return updateResult;
        }

        var newItems = request.Items
            .Select(i => ExpenseItem.Create(i.CategoryId, i.ItemAmount, i.Description))
            .ToList();

        expense.ReplaceItems(newItems);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
