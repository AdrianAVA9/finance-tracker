using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Enums;
using Fintrack.Server.Domain.Users;

namespace Fintrack.Server.Domain.Incomes;

public class RecurringIncome : BaseAuditableEntity
{
    public string UserId { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int CategoryId { get; set; }
    public RecurringFrequency Frequency { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime NextProcessingDate { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual ApplicationUser? User { get; set; }
    public virtual IncomeCategory? Category { get; set; }
}
