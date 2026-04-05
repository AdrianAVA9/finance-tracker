using Fintrack.Server.Domain.Budgets;

namespace Fintrack.Server.Domain.Budgets;

public interface IBudgetRepository
{
    // ═══════════════════════════════════════════════════════════════
    // QUERY METHODS
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Gets a budget by its unique identifier and user id.
    /// </summary>
    Task<Budget?> GetByIdAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a budget for a specific category, user, month, and year
    /// </summary>
    Task<Budget?> GetUserBudgetAsync(
        string userId,
        int categoryId,
        int month,
        int year,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all budgets for a user in a specific month and year
    /// </summary>
    Task<IReadOnlyList<Budget>> GetUserBudgetsByMonthAsync(
        string userId,
        int month,
        int year,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a budget exists for a given category, month, and year for a user
    /// </summary>
    Task<bool> ExistsAsync(
        string userId,
        int categoryId,
        int month,
        int year,
        CancellationToken cancellationToken = default);

    // ═══════════════════════════════════════════════════════════════
    // COMMAND METHODS (tracking only, no SaveChanges)
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Adds a new budget to the context
    /// </summary>
    void Add(Budget budget);

    /// <summary>
    /// Adds multiple budgets to the context
    /// </summary>
    void AddRange(IEnumerable<Budget> budgets);

    /// <summary>
    /// Updates an existing budget in the context
    /// </summary>
    void Update(Budget budget);

    /// <summary>
    /// Removes a budget from the context
    /// </summary>
    void Remove(Budget budget);

    /// <summary>
    /// Removes multiple budgets from the context
    /// </summary>
    void RemoveRange(IEnumerable<Budget> budgets);
}
