using FluentValidation;
using Fintrack.Server.Application.Abstractions.Messaging;
using Fintrack.Server.Application.Expenses;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Expenses;

namespace Fintrack.Server.Application.Expenses.Commands.CreateExpense;

public record ExpenseItemDto(Guid CategoryId, decimal ItemAmount, string? Description);

public record CreateExpenseCommand(
    string UserId,
    decimal TotalAmount,
    DateTime Date,
    string? Merchant,
    string? InvoiceNumber,
    string? InvoiceImageUrl,
    List<ExpenseItemDto> Items,
    ExpenseInvoicePayload? Invoice
) : ICommand<Guid>;

internal sealed class CreateExpenseCommandValidator : AbstractValidator<CreateExpenseCommand>
{
    public CreateExpenseCommandValidator()
    {
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

        RuleFor(x => x)
            .Must(cmd => cmd.Invoice == null
                         || ExpenseInvoicePayloadRules.Validate(cmd.Invoice, cmd.TotalAmount).IsSuccess)
            .WithName("InvoicePayload")
            .WithMessage("Invoice lines and totals must match the expense total.");
    }
}

internal sealed class CreateExpenseCommandHandler : ICommandHandler<CreateExpenseCommand, Guid>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateExpenseCommandHandler(
        IExpenseRepository expenseRepository,
        IUnitOfWork unitOfWork)
    {
        _expenseRepository = expenseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        CreateExpenseCommand request,
        CancellationToken cancellationToken)
    {
        var expenseResult = Expense.Create(
            request.UserId,
            request.TotalAmount,
            request.Date,
            request.Merchant,
            request.InvoiceNumber,
            request.InvoiceImageUrl);

        if (expenseResult.IsFailure)
        {
            return Result.Failure<Guid>(expenseResult.Error);
        }

        var expense = expenseResult.Value;

        foreach (var itemDto in request.Items)
        {
            var item = ExpenseItem.Create(
                itemDto.CategoryId,
                itemDto.ItemAmount,
                itemDto.Description);

            expense.AddItem(item);
        }

        if (request.Invoice != null)
        {
            var invoiceBuild = ExpenseInvoicePayloadRules.BuildInvoice(request.UserId, request.Invoice);
            if (invoiceBuild.IsFailure)
            {
                return Result.Failure<Guid>(invoiceBuild.Error);
            }

            expense.LinkInvoice(invoiceBuild.Value);
        }

        _expenseRepository.Add(expense);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return expense.Id;
    }
}
