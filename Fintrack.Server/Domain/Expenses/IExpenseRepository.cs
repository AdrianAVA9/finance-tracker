namespace Fintrack.Server.Domain.Expenses;

public interface IExpenseRepository
{
    Task<Expense?> GetByIdAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Expense?> GetByIdWithItemsAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Expense with expense items (and categories) plus optional linked invoice and invoice lines.
    /// </summary>
    Task<Expense?> GetByIdWithFullDetailsAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyDictionary<Guid, decimal>> SumItemAmountsByCategoryAsync(
        string userId,
        DateTime startInclusive,
        DateTime endExclusive,
        IReadOnlyCollection<Guid> categoryIds,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ExpenseItem>> GetItemsForUserCategoryInDateRangeAsync(
        string userId,
        Guid categoryId,
        DateTime startInclusive,
        DateTime endExclusive,
        CancellationToken cancellationToken = default);

    void Add(Expense expense);

    void Update(Expense expense);

    void Remove(Expense expense);
}
