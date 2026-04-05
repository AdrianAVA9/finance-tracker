namespace Fintrack.Server.Domain.Expenses;

public interface IExpenseRepository
{
    Task AddAsync(Expense expense, CancellationToken cancellationToken = default);

    Task<IReadOnlyDictionary<int, decimal>> SumItemAmountsByCategoryAsync(
        string userId,
        DateTime startInclusive,
        DateTime endExclusive,
        IReadOnlyCollection<int> categoryIds,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ExpenseItem>> GetItemsForUserCategoryInDateRangeAsync(
        string userId,
        int categoryId,
        DateTime startInclusive,
        DateTime endExclusive,
        CancellationToken cancellationToken = default);
}
