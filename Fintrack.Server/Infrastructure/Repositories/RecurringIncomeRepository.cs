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
        return await _dbContext.RecurringIncomes
            .Where(ri => ri.UserId == userId && ri.IsActive && ri.StartDate < periodEndExclusive)
            .SumAsync(ri => ri.Amount, cancellationToken);
    }
}
