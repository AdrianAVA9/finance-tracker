using Fintrack.Server.Domain.Incomes;
using Fintrack.Server.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fintrack.Server.Infrastructure.Repositories;

internal sealed class RecurringIncomeRepository : IRecurringIncomeRepository
{
    private readonly ApplicationDbContext _dbContext;

    public RecurringIncomeRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<decimal> SumActiveAmountForUserBeforeAsync(
        string userId,
        DateTime periodEndExclusive,
        CancellationToken cancellationToken = default)
    {
        // SQLite cannot translate Sum on decimal; sum in-memory (row set is small per user).
        var amounts = await _dbContext.RecurringIncomes
            .AsNoTracking()
            .Where(ri => ri.UserId == userId && ri.IsActive && ri.StartDate < periodEndExclusive)
            .Select(ri => ri.Amount)
            .ToListAsync(cancellationToken);

        return amounts.Sum();
    }
}
