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

namespace Fintrack.Server.Domain.Incomes
{
    public class Income : IAuditableEntity
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty; // Pagador
        public decimal Amount { get; set; }
        public int CategoryId { get; set; }
        public DateTime Date { get; set; }
        public string? Notes { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public virtual IncomeCategory? Category { get; set; }
    }
}
