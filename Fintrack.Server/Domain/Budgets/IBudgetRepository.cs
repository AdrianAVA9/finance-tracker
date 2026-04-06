namespace Fintrack.Server.Domain.Budgets;

public interface IBudgetRepository
{
    Task<Budget?> GetByIdAsync(
        int id,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Budget?> GetByIdWithCategoryAsync(
        int id,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Budget?> GetUserBudgetAsync(
        string userId,
        int categoryId,
        int month,
        int year,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Budget>> GetUserBudgetsByMonthAsync(
        string userId,
        int month,
        int year,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Budget>> GetUserBudgetsByMonthWithCategoryAsync(
        string userId,
        int month,
        int year,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Recurrent budgets for the given calendar month/year across all users (e.g. background roll-forward).
    /// </summary>
    Task<IReadOnlyList<Budget>> GetRecurrentBudgetsForMonthForAllUsersAsync(
        int month,
        int year,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        string userId,
        int categoryId,
        int month,
        int year,
        CancellationToken cancellationToken = default);

    void Add(Budget budget);

    void AddRange(IEnumerable<Budget> budgets);

    void Update(Budget budget);

    void Remove(Budget budget);

    void RemoveRange(IEnumerable<Budget> budgets);
}
