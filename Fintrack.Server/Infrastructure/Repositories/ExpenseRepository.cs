using System.Threading;
using System.Threading.Tasks;
using Fintrack.Server.Data;
using Fintrack.Server.Domain.Expenses;
using Fintrack.Server.Models;

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
}
