using Fintrack.Server.Domain.Expenses;
using Fintrack.Server.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fintrack.Server.Infrastructure.Repositories;

public class ExpenseRepository : IExpenseRepository
{
    private readonly ApplicationDbContext _context;

    public ExpenseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Expense expense, CancellationToken cancellationToken = default)
    {
        await _context.Expenses.AddAsync(expense, cancellationToken);
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

        var spentItems = await _context.ExpenseItems
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
        return await _context.ExpenseItems
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
}
