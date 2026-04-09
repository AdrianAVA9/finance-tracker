using FluentValidation;
using Fintrack.Server.Application.Abstractions.Messaging;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Expenses;

namespace Fintrack.Server.Application.Expenses.Queries.GetExpenseById;

public record ExpenseItemDetailsDto(
    Guid Id,
    Guid CategoryId,
    string CategoryName,
    decimal ItemAmount,
    string? Description);

public record InvoiceLineDetailsDto(
    Guid Id,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice,
    Guid? AssignedCategoryId,
    string? AssignedCategoryName);

public record ExpenseInvoiceDetailsDto(
    Guid Id,
    string? ImageUrl,
    string? MerchantName,
    DateTime? Date,
    decimal TotalAmount,
    string Status,
    List<InvoiceLineDetailsDto> Lines);

public record ExpenseDetailsDto(
    Guid Id,
    string? Merchant,
    decimal TotalAmount,
    DateTime Date,
    string StatusName,
    string? InvoiceNumber,
    string? InvoiceImageUrl,
    List<ExpenseItemDetailsDto> Items,
    ExpenseInvoiceDetailsDto? Invoice);

public record GetExpenseByIdQuery(Guid Id, string UserId) : IQuery<ExpenseDetailsDto>;

internal sealed class GetExpenseByIdQueryValidator : AbstractValidator<GetExpenseByIdQuery>
{
    public GetExpenseByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty)
            .WithMessage("Expense ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");
    }
}

internal sealed class GetExpenseByIdQueryHandler : IQueryHandler<GetExpenseByIdQuery, ExpenseDetailsDto>
{
    private readonly IExpenseRepository _expenseRepository;

    public GetExpenseByIdQueryHandler(IExpenseRepository expenseRepository)
    {
        _expenseRepository = expenseRepository;
    }

    public async Task<Result<ExpenseDetailsDto>> Handle(
        GetExpenseByIdQuery request,
        CancellationToken cancellationToken)
    {
        var expense = await _expenseRepository.GetByIdWithFullDetailsAsync(
            request.Id,
            request.UserId,
            cancellationToken);

        if (expense is null)
        {
            return Result.Failure<ExpenseDetailsDto>(ExpenseErrors.NotFound);
        }

        ExpenseInvoiceDetailsDto? invoiceDto = null;
        if (expense.Invoice != null)
        {
            var inv = expense.Invoice;
            invoiceDto = new ExpenseInvoiceDetailsDto(
                inv.Id,
                inv.ImageUrl,
                inv.MerchantName,
                inv.Date,
                inv.TotalAmount,
                inv.Status,
                inv.Items.Select(li => new InvoiceLineDetailsDto(
                    li.Id,
                    li.ProductName,
                    li.Quantity,
                    li.UnitPrice,
                    li.TotalPrice,
                    li.AssignedCategoryId,
                    li.AssignedCategory?.Name)).ToList());
        }

        var dto = new ExpenseDetailsDto(
            expense.Id,
            expense.Merchant,
            expense.TotalAmount,
            expense.Date,
            expense.Status.ToString(),
            expense.InvoiceNumber,
            expense.InvoiceImageUrl,
            expense.Items.Select(i => new ExpenseItemDetailsDto(
                i.Id,
                i.CategoryId,
                i.Category?.Name ?? "Unknown",
                i.ItemAmount,
                i.Description)).ToList(),
            invoiceDto);

        return dto;
    }
}
