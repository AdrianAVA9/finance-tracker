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


namespace Fintrack.Server.Domain.SavingsGoals
{
    public class SavingsGoal
    {        public int Id { get; set; }        public string UserId { get; set; } = string.Empty;        public string Name { get; set; } = string.Empty;        public decimal TargetAmount { get; set; }        public decimal CurrentAmount { get; set; }

        public DateTime? TargetDate { get; set; }

        public bool IsCompleted { get; set; }        public virtual ApplicationUser? User { get; set; }
    }
}
