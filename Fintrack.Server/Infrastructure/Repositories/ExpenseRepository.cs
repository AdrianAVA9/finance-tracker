using Fintrack.Server.Domain.Expenses;
using Fintrack.Server.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fintrack.Server.Infrastructure.Repositories;

internal sealed class ExpenseRepository : IExpenseRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ExpenseRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Expense?> GetByIdAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Expenses
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId, cancellationToken);
    }

    public async Task<Expense?> GetByIdWithItemsAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Expenses
            .Include(e => e.Items)
            .ThenInclude(i => i.Category)
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId, cancellationToken);
    }

    public async Task<IReadOnlyDictionary<Guid, decimal>> SumItemAmountsByCategoryAsync(
        string userId,
        DateTime startInclusive,
        DateTime endExclusive,
        IReadOnlyCollection<Guid> categoryIds,
        CancellationToken cancellationToken = default)
    {
        if (categoryIds.Count == 0)
        {
            return new Dictionary<Guid, decimal>();
        }

        var spentItems = await _dbContext.ExpenseItems
            .Include(i => i.Expense)
            .Where(i =>
                i.Expense != null &&
                i.Expense.UserId == userId &&
                i.Expense.Date >= startInclusive &&
                i.Expense.Date < endExclusive &&
                categoryIds.Contains(i.CategoryId))
            .Select(i => new { i.CategoryId, i.ItemAmount })
            .ToListAsync(cancellationToken);

        return spentItems
            .GroupBy(i => i.CategoryId)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.ItemAmount));
    }

    public async Task<IReadOnlyList<ExpenseItem>> GetItemsForUserCategoryInDateRangeAsync(
        string userId,
        Guid categoryId,
        DateTime startInclusive,
        DateTime endExclusive,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ExpenseItems
            .Include(i => i.Expense)
            .Where(i =>
                i.Expense != null &&
                i.CategoryId == categoryId &&
                i.Expense.UserId == userId &&
                i.Expense.Date >= startInclusive &&
                i.Expense.Date < endExclusive)
            .OrderByDescending(i => i.Expense!.Date)
            .ToListAsync(cancellationToken);
    }

    public void Add(Expense expense)
    {
        _dbContext.Expenses.Add(expense);
    }

    public void Update(Expense expense)
    {
        _dbContext.Expenses.Update(expense);
    }

    public void Remove(Expense expense)
    {
        _dbContext.Expenses.Remove(expense);
    }
}
