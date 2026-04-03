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

using System;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Enums;

namespace Fintrack.Server.Domain.Expenses
{
    public class RecurringExpense : BaseAuditableEntity
    {
        public string UserId { get; set; } = string.Empty;
        public string? Merchant { get; set; }
        public decimal Amount { get; set; }

        public int CategoryId { get; set; }

        public RecurringFrequency Frequency { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime NextProcessingDate { get; set; }

        public bool IsActive { get; set; } = true;
        public virtual ApplicationUser? User { get; set; }
        public virtual ExpenseCategory? Category { get; set; }
    }
}
