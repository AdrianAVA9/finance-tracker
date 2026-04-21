namespace Fintrack.Server.Domain.Incomes;

public interface IIncomeRepository
{
    Task<Income?> GetByIdAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Income?> GetByIdAsNoTrackingAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default);

    void Add(Income income);

    void Update(Income income);

    void Remove(Income income);

    Task<decimal> SumAmountForUserInPeriodAsync(
        string userId,
        DateTime start,
        DateTime end,
        CancellationToken cancellationToken = default);
}
