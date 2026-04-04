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

namespace Fintrack.Server.Domain.ExpenseCategories
{
    public interface IExpenseCategoryGroupRepository
    {
        Task<ExpenseCategoryGroup?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ExpenseCategoryGroup>> GetAllByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
        void Add(ExpenseCategoryGroup group);
        void Update(ExpenseCategoryGroup group);
        void Remove(ExpenseCategoryGroup group);
    }
}
