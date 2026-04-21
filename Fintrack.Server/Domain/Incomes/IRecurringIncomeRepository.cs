namespace Fintrack.Server.Domain.Incomes;

public interface IRecurringIncomeRepository
{
    /// <summary>
    /// Sums active recurring incomes for the user where <see cref="RecurringIncome.StartDate"/> is before <paramref name="periodEndExclusive"/>.
    /// </summary>
    Task<decimal> SumActiveAmountForUserBeforeAsync(
        string userId,
        DateTime periodEndExclusive,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds an active recurring template matching legacy sync semantics (same user, source, category, and amount).
    /// </summary>
    Task<RecurringIncome?> FindActiveMatchingTemplateAsync(
        string userId,
        string source,
        Guid categoryId,
        decimal amount,
        CancellationToken cancellationToken = default);

    void Add(RecurringIncome recurringIncome);
}
