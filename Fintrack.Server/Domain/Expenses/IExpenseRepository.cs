using System.Threading;
using System.Threading.Tasks;

namespace Fintrack.Server.Domain.Expenses;

public interface IExpenseRepository
{
    Task AddAsync(Expense expense, CancellationToken cancellationToken = default);
}
