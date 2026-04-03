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

using Microsoft.AspNetCore.Identity;

namespace Fintrack.Server.Domain.Users
{
    public class ApplicationUser : IdentityUser
    {
        public string? Currency { get; set; } = "USD";
        public string? Country { get; set; }

        // Setting a default value as specified in the story
        public int FinancialMonthStart { get; set; } = 1;

        // Navigation properties
        public virtual ICollection<ExpenseCategory> ExpenseCategories { get; set; } = new List<ExpenseCategory>();
        public virtual ICollection<IncomeCategory> IncomeCategories { get; set; } = new List<IncomeCategory>();
        public virtual ICollection<Income> Incomes { get; set; } = new List<Income>();
        public virtual ICollection<RecurringIncome> RecurringIncomes { get; set; } = new List<RecurringIncome>();
        public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
        public virtual ICollection<Budget> Budgets { get; set; } = new List<Budget>();
        public virtual ICollection<SavingsGoal> SavingsGoals { get; set; } = new List<SavingsGoal>();
    }
}
