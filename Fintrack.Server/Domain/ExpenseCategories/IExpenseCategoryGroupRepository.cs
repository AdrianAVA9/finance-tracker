using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Fintrack.Server.Domain.ExpenseCategories;

public interface IExpenseCategoryGroupRepository
{
    Task<ExpenseCategoryGroup?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ExpenseCategoryGroup>> GetAllByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(ExpenseCategoryGroup group);
    void Update(ExpenseCategoryGroup group);
    void Remove(ExpenseCategoryGroup group);
}
