using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Budgets;
using Fintrack.Server.Domain.Enums;
using Fintrack.Server.Domain.Exceptions;
using Fintrack.Server.Domain.ExpenseCategories;
using Fintrack.Server.Domain.Expenses;
using Fintrack.Server.Domain.Incomes;
using Fintrack.Server.Domain.Invoices;
using Fintrack.Server.Domain.SavingsGoals;
using Fintrack.Server.Domain.Users;

using Fintrack.Server.Domain.Abstractions;

namespace Fintrack.Server.Domain.ExpenseCategories
{
    public class ExpenseCategoryGroup : IAuditableEntity
    {
        public int Id { get; set; }

        public string? UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public bool IsEditable { get; set; }

        // Audit Columns
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public virtual ICollection<ExpenseCategory> Categories { get; set; } = new List<ExpenseCategory>();
    }
}
