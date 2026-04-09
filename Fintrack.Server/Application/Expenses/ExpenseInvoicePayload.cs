using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Expenses;
using Fintrack.Server.Domain.Invoices;

namespace Fintrack.Server.Application.Expenses;

public record ExpenseInvoiceLinePayload(
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice,
    Guid? AssignedCategoryId);

public record ExpenseInvoicePayload(
    string? ImageUrl,
    string? MerchantName,
    DateTime? Date,
    decimal TotalAmount,
    string? Status,
    IReadOnlyList<ExpenseInvoiceLinePayload> Lines);

public static class ExpenseInvoicePayloadRules
{
    private const decimal Tolerance = 0.01m;

    public static Result Validate(ExpenseInvoicePayload invoice, decimal expenseTotalAmount)
    {
        if (invoice.Lines.Count == 0)
        {
            return Result.Failure(ExpenseErrors.InvoicePayloadInvalid);
        }

        if (invoice.TotalAmount < 0)
        {
            return Result.Failure(ExpenseErrors.InvoicePayloadInvalid);
        }

        if (Math.Abs(invoice.TotalAmount - expenseTotalAmount) > Tolerance)
        {
            return Result.Failure(ExpenseErrors.InvoicePayloadInvalid);
        }

        var sumLines = invoice.Lines.Sum(l => l.TotalPrice);
        if (Math.Abs(sumLines - invoice.TotalAmount) > Tolerance)
        {
            return Result.Failure(ExpenseErrors.InvoicePayloadInvalid);
        }

        foreach (var line in invoice.Lines)
        {
            if (string.IsNullOrWhiteSpace(line.ProductName))
            {
                return Result.Failure(ExpenseErrors.InvoicePayloadInvalid);
            }

            if (line.Quantity <= 0)
            {
                return Result.Failure(ExpenseErrors.InvoicePayloadInvalid);
            }

            if (line.UnitPrice < 0 || line.TotalPrice < 0)
            {
                return Result.Failure(ExpenseErrors.InvoicePayloadInvalid);
            }
        }

        return Result.Success();
    }

    public static Result<Invoice> BuildInvoice(string userId, ExpenseInvoicePayload payload)
    {
        var status = string.IsNullOrWhiteSpace(payload.Status) ? "Completed" : payload.Status.Trim();

        var invoiceResult = Invoice.Create(
            userId,
            payload.TotalAmount,
            payload.ImageUrl,
            payload.MerchantName,
            payload.Date,
            status);

        if (invoiceResult.IsFailure)
        {
            return Result.Failure<Invoice>(invoiceResult.Error);
        }

        var invoice = invoiceResult.Value;

        foreach (var line in payload.Lines)
        {
            var name = line.ProductName.Trim();
            var itemResult = InvoiceItem.Create(
                name,
                line.Quantity,
                line.UnitPrice,
                line.TotalPrice,
                line.AssignedCategoryId);

            if (itemResult.IsFailure)
            {
                return Result.Failure<Invoice>(itemResult.Error);
            }

            invoice.AddItem(itemResult.Value);
        }

        return invoice;
    }
}
