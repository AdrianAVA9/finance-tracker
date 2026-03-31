using MediatR;
using Fintrack.Server.Data;
using Fintrack.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Fintrack.Server.Application.Budgets.Commands;

public record BudgetEntryDto(int CategoryId, decimal Amount, bool IsRecurrent = false);

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
            .ToDictionaryAsync(b => b.CategoryId, cancellationToken);

        // Group to handle safety if multiple items for same CategoryId are sent in one batch
        var uniqueBudgets = request.Budgets
            .GroupBy(b => b.CategoryId)
            .Select(g => g.Last()) // Take the last one if duplicates exist
            .ToList();

        foreach (var entry in uniqueBudgets)
        {
            if (existingBudgets.TryGetValue(entry.CategoryId, out var existing))
            {
                existing.Amount = entry.Amount;
                existing.IsRecurrent = entry.IsRecurrent;
            }
            else
            {
                _dbContext.Budgets.Add(new Budget
                {
                    UserId = request.UserId,
                    CategoryId = entry.CategoryId,
                    Amount = entry.Amount,
                    IsRecurrent = entry.IsRecurrent,
                    Month = request.Month,
                    Year = request.Year
                });
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
