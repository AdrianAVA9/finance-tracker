using System;
using System.Collections.Generic;
using System.Linq;
using Fintrack.Server.Application.Expenses.Commands;
using Fintrack.Server.Domain.Enums;

namespace Fintrack.Server.Api.Controllers.Expenses;

public sealed record RequestExpenseItemData(int CategoryId, decimal ItemAmount, string? Description);

public sealed class RequestCreateExpense
{
    public string? UserId { get; set; }
    public required string Merchant { get; init; }
    public decimal TotalAmount { get; init; }
    public DateTime Date { get; init; }
    public string? InvoiceNumber { get; init; }
    public string? InvoiceImageUrl { get; init; }
    public bool IsRecurring { get; init; }
    public RecurringFrequency? Frequency { get; init; }
    public required List<RequestExpenseItemData> Items { get; init; } = new();

    public CreateExpenseCommand ToCommand()
    {
        return new CreateExpenseCommand(
            UserId,
            TotalAmount,
            Date,
            Merchant,
            InvoiceNumber,
            InvoiceImageUrl,
            IsRecurring,
            Frequency,
            Items.Select(i => new ExpenseItemDto(i.CategoryId, i.ItemAmount, i.Description)).ToList()
        );
    }
}
