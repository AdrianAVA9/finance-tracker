namespace Fintrack.Server.Domain.Expenses;

public interface IExpenseRepository
{
    Task AddAsync(Expense expense, CancellationToken cancellationToken = default);

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
}
