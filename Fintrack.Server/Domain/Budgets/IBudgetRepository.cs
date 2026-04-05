namespace Fintrack.Server.Domain.Budgets;

public interface IBudgetRepository
{
    Task<Budget?> GetByIdAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Budget?> GetByIdWithCategoryAsync(
        Guid id,
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
    /// Budgets marked recurrent for the given calendar month/year (any amount of rows).
    /// </summary>
    Task<IReadOnlyList<Budget>> GetRecurrentBudgetsForMonthAsync(
        string userId,
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
