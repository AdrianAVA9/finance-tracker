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


namespace Fintrack.Server.Domain.Invoices
{
    public class InvoiceItem
    {        public int Id { get; set; }        public int InvoiceId { get; set; }        public string ProductName { get; set; } = string.Empty;

        public int Quantity { get; set; }        public decimal UnitPrice { get; set; }        public decimal TotalPrice { get; set; }

        // Nullable category ID if auto-categorized
        public int? AssignedCategoryId { get; set; }        public virtual Invoice? Invoice { get; set; }        public virtual ExpenseCategory? AssignedCategory { get; set; }
    }
}
