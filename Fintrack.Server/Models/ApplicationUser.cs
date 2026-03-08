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
        public virtual ICollection<IncomeSource> IncomeSources { get; set; } = new List<IncomeSource>();
        public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
        public virtual ICollection<Budget> Budgets { get; set; } = new List<Budget>();
        public virtual ICollection<SavingsGoal> SavingsGoals { get; set; } = new List<SavingsGoal>();
    }
}
