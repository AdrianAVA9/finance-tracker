using Fintrack.Server.Domain.Incomes;
using Fintrack.Server.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fintrack.Server.Infrastructure.Repositories;

internal sealed class IncomeRepository : IIncomeRepository
{
    private readonly ApplicationDbContext _dbContext;

    public IncomeRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Income?> GetByIdAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Incomes
            .FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId, cancellationToken);
    }

    public async Task<Income?> GetByIdAsNoTrackingAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Incomes
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId, cancellationToken);
    }

    public void Add(Income income)
    {
        _dbContext.Incomes.Add(income);
    }

    public void Update(Income income)
    {
        _dbContext.Incomes.Update(income);
    }

    public void Remove(Income income)
    {
        _dbContext.Incomes.Remove(income);
    }
    
    public async Task<decimal> SumAmountForUserInPeriodAsync(
        string userId,
        DateTime start,
        DateTime end,
        CancellationToken cancellationToken = default)
    {
        // For Postgres, we can sum directly. 
        // For consistency with RecurringIncomeRepository (SQLite compatibility in tests), we sum in-memory if needed,
        // but here we'll use SumAsync since this is likely always used with a Real provider in production.
        return await _dbContext.Incomes
            .AsNoTracking()
            .Where(i => i.UserId == userId && i.Date >= start && i.Date < end)
            .SumAsync(i => i.Amount, cancellationToken);
    }
}
