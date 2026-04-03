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


namespace Fintrack.Server.Domain.Budgets
{
    public class Budget
    {        public int Id { get; set; }        public string UserId { get; set; } = string.Empty;        public int CategoryId { get; set; }        public decimal Amount { get; set; }

        public bool IsRecurrent { get; set; } = false;        public int Month { get; set; }        public int Year { get; set; }        public virtual ApplicationUser? User { get; set; }        public virtual ExpenseCategory? Category { get; set; }
    }
}
