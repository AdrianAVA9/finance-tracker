using MediatR;
using Fintrack.Server.Data;
using Microsoft.EntityFrameworkCore;
using Fintrack.Server.Models;


namespace Fintrack.Server.Application.Budgets.Commands;

public record DeleteBudgetCommand(int Id, string UserId) : IRequest<Unit>;

internal sealed class DeleteBudgetCommandHandler : IRequestHandler<DeleteBudgetCommand, Unit>
{
    private readonly ApplicationDbContext _dbContext;

    public DeleteBudgetCommandHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Unit> Handle(DeleteBudgetCommand request, CancellationToken cancellationToken)
    {
        var budget = await _dbContext.Budgets
            .FirstOrDefaultAsync(b => b.Id == request.Id && b.UserId == request.UserId, cancellationToken);

        if (budget != null)
        {
            _dbContext.Budgets.Remove(budget);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return Unit.Value;
    }
}
