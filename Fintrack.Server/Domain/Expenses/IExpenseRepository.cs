using System.Threading;
using System.Threading.Tasks;
using Fintrack.Server.Models;

namespace Fintrack.Server.Domain.Expenses;

public interface IExpenseRepository
{
    Task AddAsync(Expense expense, CancellationToken cancellationToken = default);
}
