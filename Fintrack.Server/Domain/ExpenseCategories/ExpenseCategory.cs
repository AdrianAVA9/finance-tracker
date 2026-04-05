using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Budgets;
using Fintrack.Server.Domain.Expenses;
using Fintrack.Server.Domain.Users;

namespace Fintrack.Server.Domain.ExpenseCategories;

public class ExpenseCategory : BaseAuditableEntity
{
    public string? UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Color { get; set; }
    public string? Icon { get; set; }
    public string? Description { get; set; }
    public int? GroupId { get; set; }
    public bool IsEditable { get; set; }

    public virtual ApplicationUser? User { get; set; }
    public virtual ExpenseCategoryGroup? Group { get; set; }
    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    public virtual ICollection<Budget> Budgets { get; set; } = new List<Budget>();
}
