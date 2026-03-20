using Fintrack.Server.Models;

namespace Fintrack.Server.Domain.ExpenseCategories
{
    public interface IExpenseCategoryRepository
    {
        Task<ExpenseCategory?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ExpenseCategory>> GetAllByUserIdAsync(
            string userId,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(
            int id,
             CancellationToken cancellationToken = default);

        void Add(ExpenseCategory expenseCategory);

        void Update(ExpenseCategory expenseCategory);

        void Remove(ExpenseCategory expenseCategory);
    }
}
