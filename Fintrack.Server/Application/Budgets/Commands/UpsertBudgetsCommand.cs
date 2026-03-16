using MediatR;
using Fintrack.Server.Data;
using Fintrack.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Fintrack.Server.Application.Budgets.Commands;

public record BudgetEntryDto(int CategoryId, decimal Amount);

public record UpsertBudgetsCommand(
    string UserId,
    int Month,
    int Year,
    List<BudgetEntryDto> Budgets
) : IRequest<Unit>;

internal sealed class UpsertBudgetsCommandHandler : IRequestHandler<UpsertBudgetsCommand, Unit>
{
    private readonly ApplicationDbContext _dbContext;

    public UpsertBudgetsCommandHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Unit> Handle(UpsertBudgetsCommand request, CancellationToken cancellationToken)
    {
        var existingBudgets = await _dbContext.Budgets
            .Where(b => b.UserId == request.UserId && b.Month == request.Month && b.Year == request.Year)
            .ToListAsync(cancellationToken);

        foreach (var entry in request.Budgets)
        {
            var existing = existingBudgets.FirstOrDefault(b => b.CategoryId == entry.CategoryId);

            if (existing != null)
            {
                existing.Amount = entry.Amount;
            }
            else
            {
                _dbContext.Budgets.Add(new Budget
                {
                    UserId = request.UserId,
                    CategoryId = entry.CategoryId,
                    Amount = entry.Amount,
                    Month = request.Month,
                    Year = request.Year
                });
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
