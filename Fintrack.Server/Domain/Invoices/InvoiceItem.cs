using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.ExpenseCategories;

namespace Fintrack.Server.Domain.Invoices;

public sealed class InvoiceItem : BaseAuditableEntityGuid
{
    public Guid InvoiceId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TotalPrice { get; private set; }
    public Guid? AssignedCategoryId { get; private set; }

    public Invoice? Invoice { get; private set; }
    public ExpenseCategory? AssignedCategory { get; private set; }

    private InvoiceItem() : base()
    {
    }

    private InvoiceItem(
        string productName,
        int quantity,
        decimal unitPrice,
        decimal totalPrice,
        Guid? assignedCategoryId)
        : base()
    {
        Id = Guid.NewGuid();
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
        TotalPrice = totalPrice;
        AssignedCategoryId = assignedCategoryId;
    }

    public static Result<InvoiceItem> Create(
        string productName,
        int quantity,
        decimal unitPrice,
        decimal totalPrice,
        Guid? assignedCategoryId = null)
    {
        if (string.IsNullOrWhiteSpace(productName))
        {
            return Result.Failure<InvoiceItem>(InvoiceItemErrors.ProductNameRequired);
        }

        if (quantity <= 0)
        {
            return Result.Failure<InvoiceItem>(InvoiceItemErrors.InvalidQuantity);
        }

        if (unitPrice < 0)
        {
            return Result.Failure<InvoiceItem>(InvoiceItemErrors.NegativeUnitPrice);
        }

        if (totalPrice < 0)
        {
            return Result.Failure<InvoiceItem>(InvoiceItemErrors.NegativeTotalPrice);
        }

        return new InvoiceItem(
            productName.Trim(),
            quantity,
            unitPrice,
            totalPrice,
            assignedCategoryId);
    }
}
