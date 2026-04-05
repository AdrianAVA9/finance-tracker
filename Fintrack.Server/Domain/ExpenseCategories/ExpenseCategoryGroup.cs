using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Users;

namespace Fintrack.Server.Domain.ExpenseCategories;

public class ExpenseCategoryGroup : BaseAuditableEntity
{
    public string? UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsEditable { get; set; }

    public virtual ApplicationUser? User { get; set; }
    public virtual ICollection<ExpenseCategory> Categories { get; set; } = new List<ExpenseCategory>();
}
