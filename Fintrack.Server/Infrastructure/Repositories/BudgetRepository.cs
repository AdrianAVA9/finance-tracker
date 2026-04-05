using Fintrack.Server.Domain.Budgets;
using Fintrack.Server.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fintrack.Server.Infrastructure.Repositories;

internal sealed class BudgetRepository : IBudgetRepository
{
    private readonly ApplicationDbContext _dbContext;

    public BudgetRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Budget?> GetByIdAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Budgets
            .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId, cancellationToken);
    }

    public async Task<Budget?> GetByIdWithCategoryAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Budgets
            .Include(b => b.Category)
            .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId, cancellationToken);
    }

    public async Task<Budget?> GetUserBudgetAsync(
        string userId,
        int categoryId,
        int month,
        int year,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Budgets
            .FirstOrDefaultAsync(b =>
                b.UserId == userId &&
                b.CategoryId == categoryId &&
                b.Month == month &&
                b.Year == year,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Budget>> GetUserBudgetsByMonthAsync(
        string userId,
        int month,
        int year,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Budgets
            .Where(b => b.UserId == userId && b.Month == month && b.Year == year)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Budget>> GetUserBudgetsByMonthWithCategoryAsync(
        string userId,
        int month,
        int year,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Budgets
            .Include(b => b.Category)
            .ThenInclude(c => c!.Group)
            .Where(b => b.UserId == userId && b.Month == month && b.Year == year)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Budget>> GetRecurrentBudgetsForMonthAsync(
        string userId,
        int month,
        int year,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Budgets
            .Where(b =>
                b.UserId == userId &&
                b.IsRecurrent &&
                b.Month == month &&
                b.Year == year)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        string userId,
        int categoryId,
        int month,
        int year,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Budgets
            .AnyAsync(b =>
                b.UserId == userId &&
                b.CategoryId == categoryId &&
                b.Month == month &&
                b.Year == year,
                cancellationToken);
    }

    public void Add(Budget budget)
    {
        _dbContext.Budgets.Add(budget);
    }

    public void AddRange(IEnumerable<Budget> budgets)
    {
        _dbContext.Budgets.AddRange(budgets);
    }

    public void Update(Budget budget)
    {
        _dbContext.Budgets.Update(budget);
    }

    public void Remove(Budget budget)
    {
        _dbContext.Budgets.Remove(budget);
    }

    public void RemoveRange(IEnumerable<Budget> budgets)
    {
        _dbContext.Budgets.RemoveRange(budgets);
    }
}
