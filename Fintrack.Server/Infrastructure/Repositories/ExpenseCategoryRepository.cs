using Microsoft.EntityFrameworkCore;
using Fintrack.Server.Data;
using Fintrack.Server.Domain.ExpenseCategories;
using Fintrack.Server.Models;

namespace Fintrack.Server.Infrastructure.Repositories
{
    internal sealed class ExpenseCategoryRepository : IExpenseCategoryRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ExpenseCategoryRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ExpenseCategory?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext
                .ExpenseCategories
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<ExpenseCategory>> GetAllByUserIdAsync(
            string userId,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext
                .ExpenseCategories
                .Include(e => e.Group)
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
                .ExpenseCategories
                .AnyAsync(e => e.Id == id, cancellationToken);
        }

        public void Add(ExpenseCategory expenseCategory)
        {
            _dbContext.ExpenseCategories.Add(expenseCategory);
        }

        public void Update(ExpenseCategory expenseCategory)
        {
            _dbContext.ExpenseCategories.Update(expenseCategory);
        }

        public void Remove(ExpenseCategory expenseCategory)
        {
            _dbContext.ExpenseCategories.Remove(expenseCategory);
        }
    }
}
