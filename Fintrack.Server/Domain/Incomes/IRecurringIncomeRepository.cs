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
}
