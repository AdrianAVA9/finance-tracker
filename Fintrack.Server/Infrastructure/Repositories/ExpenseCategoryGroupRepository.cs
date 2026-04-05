using Microsoft.EntityFrameworkCore;
using Fintrack.Server.Infrastructure.Data;
using Fintrack.Server.Domain.ExpenseCategories;

namespace Fintrack.Server.Infrastructure.Repositories;

public sealed class ExpenseCategoryGroupRepository : IExpenseCategoryGroupRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ExpenseCategoryGroupRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ExpenseCategoryGroup?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext
            .ExpenseCategoryGroups
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<ExpenseCategoryGroup>> GetAllByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext
            .ExpenseCategoryGroups
            .AsNoTracking()
            .Where(e => e.UserId == userId || e.UserId == null)
            .OrderBy(e => e.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext
            .ExpenseCategoryGroups
            .AnyAsync(e => e.Id == id, cancellationToken);
    }

    public void Add(ExpenseCategoryGroup group)
    {
        _dbContext.ExpenseCategoryGroups.Add(group);
    }

    public void Update(ExpenseCategoryGroup group)
    {
        _dbContext.ExpenseCategoryGroups.Update(group);
    }

    public void Remove(ExpenseCategoryGroup group)
    {
        _dbContext.ExpenseCategoryGroups.Remove(group);
    }
}
