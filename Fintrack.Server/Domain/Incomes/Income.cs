using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.IncomeCategories;
using Fintrack.Server.Domain.Users;

namespace Fintrack.Server.Domain.Incomes;

public class Income : BaseAuditableEntity
{
    public string UserId { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public Guid CategoryId { get; set; }
    public DateTime Date { get; set; }
    public string? Notes { get; set; }

    public virtual ApplicationUser? User { get; set; }
    public virtual IncomeCategory? Category { get; set; }
}
