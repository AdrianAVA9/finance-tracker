using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Fintrack.Server.Domain.ExpenseCategories;

public interface IExpenseCategoryRepository
{
    Task<ExpenseCategory?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ExpenseCategory>> GetAllByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>Categories created by the user (excludes system-seeded rows with <c>UserId == null</c>).</summary>
    Task<IReadOnlyList<ExpenseCategory>> GetUserOwnedByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    void Add(ExpenseCategory expenseCategory);

    void Update(ExpenseCategory expenseCategory);

    void Remove(ExpenseCategory expenseCategory);
}
