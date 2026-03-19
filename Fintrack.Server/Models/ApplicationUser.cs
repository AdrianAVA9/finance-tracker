using Microsoft.AspNetCore.Identity;

namespace Fintrack.Server.Models
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
