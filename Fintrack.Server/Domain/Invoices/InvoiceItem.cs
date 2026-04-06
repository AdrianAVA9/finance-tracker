using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.ExpenseCategories;

namespace Fintrack.Server.Domain.Invoices;

public class InvoiceItem : Entity
{
    public int InvoiceId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public Guid? AssignedCategoryId { get; set; }

    public virtual Invoice? Invoice { get; set; }
    public virtual ExpenseCategory? AssignedCategory { get; set; }
}
