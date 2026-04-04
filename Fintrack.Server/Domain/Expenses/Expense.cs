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
using System.Collections.Generic;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Enums;

namespace Fintrack.Server.Domain.Expenses
{
    public class Expense : BaseAuditableEntity
    {        public string UserId { get; set; } = string.Empty;        public decimal TotalAmount { get; set; }

        public DateTime Date { get; set; }        public string? Merchant { get; set; }        public string? InvoiceNumber { get; set; }        public string? InvoiceImageUrl { get; set; }

        public AiExtractionStatus AiExtractionStatus { get; set; } = AiExtractionStatus.NotApplicable;

        public ExpenseStatus Status { get; set; } = ExpenseStatus.Draft;

        public int? InvoiceId { get; set; }        public virtual ApplicationUser? User { get; set; }        public virtual Invoice? Invoice { get; set; }

        public virtual ICollection<ExpenseItem> Items { get; set; } = new List<ExpenseItem>();
    }
}
