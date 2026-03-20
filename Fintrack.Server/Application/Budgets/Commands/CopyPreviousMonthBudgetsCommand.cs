using MediatR;
using Fintrack.Server.Data;
using Fintrack.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Fintrack.Server.Application.Budgets.Commands;

public record CopyPreviousMonthBudgetsCommand(
    string UserId,
    int TargetMonth,
    int TargetYear
) : IRequest<Unit>;

internal sealed class CopyPreviousMonthBudgetsCommandHandler : IRequestHandler<CopyPreviousMonthBudgetsCommand, Unit>
{
    private readonly ApplicationDbContext _dbContext;

    public CopyPreviousMonthBudgetsCommandHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Unit> Handle(CopyPreviousMonthBudgetsCommand request, CancellationToken cancellationToken)
    {
        // Calculate previous month and year
        int prevMonth = request.TargetMonth - 1;
        int prevYear = request.TargetYear;

        if (prevMonth == 0)
        {
            prevMonth = 12;
            prevYear--;
        }

        // 1. Fetch source budgets
        var sourceBudgets = await _dbContext.Budgets
            .Where(b => b.UserId == request.UserId && b.Month == prevMonth && b.Year == prevYear)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        if (!sourceBudgets.Any())
        {
            return Unit.Value; // Nothing to copy
        }

        // 2. Clear current target month if exists (optional strategy, here we Upsert/Merge)
        var existingTargetBudgets = await _dbContext.Budgets
            .Where(b => b.UserId == request.UserId && b.Month == request.TargetMonth && b.Year == request.TargetYear)
            .ToListAsync(cancellationToken);

        foreach (var source in sourceBudgets)
        {
            var existing = existingTargetBudgets.FirstOrDefault(b => b.CategoryId == source.CategoryId);
            
            if (existing != null)
            {
                existing.Amount = source.Amount;
            }
            else
            {
                _dbContext.Budgets.Add(new Budget
                {
                    UserId = request.UserId,
                    CategoryId = source.CategoryId,
                    Amount = source.Amount,
                    Month = request.TargetMonth,
                    Year = request.TargetYear
                });
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
