using Fintrack.Server.Models;

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
