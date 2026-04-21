using Fintrack.Server.Domain.IncomeCategories;
using Fintrack.Server.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fintrack.Server.Infrastructure.Repositories;

internal sealed class IncomeCategoryRepository : IIncomeCategoryRepository
{
    private readonly ApplicationDbContext _dbContext;

    public IncomeCategoryRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IncomeCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.IncomeCategories
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<IncomeCategory>> GetAllByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.IncomeCategories
            .AsNoTracking()
            .Where(c => c.UserId == null || c.UserId == userId)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.IncomeCategories
            .AnyAsync(c => c.Id == id, cancellationToken);
    }

    public void Add(IncomeCategory incomeCategory)
    {
        _dbContext.IncomeCategories.Add(incomeCategory);
    }

    public void Update(IncomeCategory incomeCategory)
    {
        _dbContext.IncomeCategories.Update(incomeCategory);
    }

    public void Remove(IncomeCategory incomeCategory)
    {
        _dbContext.IncomeCategories.Remove(incomeCategory);
    }
}
