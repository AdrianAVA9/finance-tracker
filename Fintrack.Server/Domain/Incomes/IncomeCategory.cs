using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Users;

namespace Fintrack.Server.Domain.Incomes;

public class IncomeCategory : BaseAuditableEntity
{
    public string? UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? Color { get; set; }

    public virtual ApplicationUser? User { get; set; }
    public virtual ICollection<Income> Incomes { get; set; } = new List<Income>();
    public virtual ICollection<RecurringIncome> RecurringIncomes { get; set; } = new List<RecurringIncome>();
}
