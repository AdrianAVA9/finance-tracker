using MediatR;
using Fintrack.Server.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Budgets;
using Fintrack.Server.Domain.Enums;
using Fintrack.Server.Domain.Exceptions;
using Fintrack.Server.Domain.ExpenseCategories;
using Fintrack.Server.Domain.Expenses;
using Fintrack.Server.Domain.Incomes;
using Fintrack.Server.Domain.Invoices;
using Fintrack.Server.Domain.SavingsGoals;
using Fintrack.Server.Domain.Users;


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
