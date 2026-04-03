using MediatR;
using Fintrack.Server.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fintrack.Server.Application.Expenses.Commands
{
    public record DeleteExpenseCommand(int Id, string UserId) : IRequest<bool>;

    public class DeleteExpenseCommandHandler : IRequestHandler<DeleteExpenseCommand, bool>
    {
        private readonly ApplicationDbContext _dbContext;

        public DeleteExpenseCommandHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(DeleteExpenseCommand request, CancellationToken cancellationToken)
        {
            var expense = await _dbContext.Expenses
                .Include(e => e.Items)
                .FirstOrDefaultAsync(e => e.Id == request.Id && e.UserId == request.UserId, cancellationToken);

            if (expense == null) return false;

            // Cascade delete items and the expense record itself
            // If the user has explicitly requested hard-deletes, we remove everything.
            _dbContext.ExpenseItems.RemoveRange(expense.Items);
            _dbContext.Expenses.Remove(expense);
            
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
