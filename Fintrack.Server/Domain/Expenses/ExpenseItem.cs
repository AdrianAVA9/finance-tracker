using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.ExpenseCategories;

namespace Fintrack.Server.Domain.Expenses;

public class ExpenseItem : BaseAuditableEntity
{
    public int ExpenseId { get; set; }
    public Guid CategoryId { get; set; }
    public decimal ItemAmount { get; set; }
    public string? Description { get; set; }

    public virtual Expense? Expense { get; set; }
    public virtual ExpenseCategory? Category { get; set; }
}
