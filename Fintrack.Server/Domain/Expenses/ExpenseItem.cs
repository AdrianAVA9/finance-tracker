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

namespace Fintrack.Server.Domain.Expenses
{
    public class ExpenseItem : BaseAuditableEntity
    {
        public int ExpenseId { get; set; }

        public int CategoryId { get; set; }
        public decimal ItemAmount { get; set; }
        public string? Description { get; set; }
        public virtual Expense? Expense { get; set; }
        public virtual ExpenseCategory? Category { get; set; }
    }
}
