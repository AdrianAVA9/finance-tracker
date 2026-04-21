using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Enums;
using Fintrack.Server.Domain.ExpenseCategories;
using Fintrack.Server.Domain.Users;

namespace Fintrack.Server.Domain.Expenses;

public class RecurringExpense : BaseAuditableEntity
{
    public string UserId { get; set; } = string.Empty;
    public string? Merchant { get; set; }
    public decimal Amount { get; set; }
    public Guid CategoryId { get; set; }
    public RecurringFrequency Frequency { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime NextProcessingDate { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual ApplicationUser? User { get; set; }
    public virtual ExpenseCategory? Category { get; set; }
}
