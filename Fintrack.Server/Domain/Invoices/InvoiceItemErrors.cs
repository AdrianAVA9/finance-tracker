using Fintrack.Server.Domain.Abstractions;

namespace Fintrack.Server.Domain.Invoices;

public static class InvoiceItemErrors
{
    public static readonly Error ProductNameRequired = new(
        "InvoiceItem.ProductNameRequired",
        "Product name is required");

    public static readonly Error InvalidQuantity = new(
        "InvoiceItem.InvalidQuantity",
        "Quantity must be greater than zero");

    public static readonly Error NegativeUnitPrice = new(
        "InvoiceItem.NegativeUnitPrice",
        "Unit price cannot be negative");

    public static readonly Error NegativeTotalPrice = new(
        "InvoiceItem.NegativeTotalPrice",
        "Total price cannot be negative");
}
