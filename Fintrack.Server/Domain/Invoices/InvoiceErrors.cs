using Fintrack.Server.Domain.Abstractions;

namespace Fintrack.Server.Domain.Invoices;

public static class InvoiceErrors
{
    public static readonly Error UserIdRequired = new(
        "Invoice.UserIdRequired",
        "User ID is required");

    public static readonly Error NegativeTotalAmount = new(
        "Invoice.NegativeTotalAmount",
        "The invoice total amount cannot be negative");

    public static readonly Error StatusRequired = new(
        "Invoice.StatusRequired",
        "Invoice status is required");
}
