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
    public class Invoice
    {        public int Id { get; set; }        public string UserId { get; set; } = string.Empty;        public string? ImageUrl { get; set; }        public string? MerchantName { get; set; }

        public DateTime? Date { get; set; }        public decimal TotalAmount { get; set; }        public string Status { get; set; } = "Pending"; // Pending, Processed, Failed        public string? ErrorMessage { get; set; }        public virtual ApplicationUser? User { get; set; }

        public virtual ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
    }
}
