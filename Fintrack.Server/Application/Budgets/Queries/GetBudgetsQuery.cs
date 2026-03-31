using System;
using MediatR;
using Fintrack.Server.Data;
using Microsoft.EntityFrameworkCore;
using Fintrack.Server.Models;


namespace Fintrack.Server.Application.Budgets.Queries;

public record BudgetDto(
    int Id,
    int CategoryId,
    string CategoryName,
    string? CategoryIcon,
    string? CategoryColor,
    string? CategoryGroup,
    decimal LimitAmount,
    decimal SpentAmount,
    bool IsRecurrent
);

public record BudgetListDto(
    List<BudgetDto> Budgets,
    decimal MonthlyIncome
);

public record GetBudgetsQuery(
    string UserId,
    int Month,
    int Year
) : IRequest<BudgetListDto>;

internal sealed class GetBudgetsQueryHandler : IRequestHandler<GetBudgetsQuery, BudgetListDto>
{
    private readonly ApplicationDbContext _dbContext;

    public GetBudgetsQueryHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<BudgetListDto> Handle(GetBudgetsQuery request, CancellationToken cancellationToken)
    {
        // 0. Fetch total expected Income for the month (based on recurring income)
        var startDate = new DateTime(request.Year, request.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = startDate.AddMonths(1);

        var monthlyIncome = await _dbContext.RecurringIncomes
            .Where(ri => ri.UserId == request.UserId && 
                        ri.IsActive && 
                        ri.StartDate < endDate)
            .SumAsync(ri => ri.Amount, cancellationToken);

        // 1. Fetch registered budgets for the month
        var userBudgets = await _dbContext.Budgets
            .Include(b => b.Category)
            .ThenInclude(c => c.Group)
            .Where(b => b.UserId == request.UserId && b.Month == request.Month && b.Year == request.Year)
            .ToListAsync(cancellationToken);

        // 1.1 Auto-copy recurrent budgets if none exist for this month
        if (!userBudgets.Any())
        {
            var prevDate = startDate.AddMonths(-1);
            var recurrentBudgets = await _dbContext.Budgets
                .Where(b => b.UserId == request.UserId && 
                            b.IsRecurrent && 
                            b.Month == prevDate.Month && 
                            b.Year == prevDate.Year)
                .ToListAsync(cancellationToken);

            if (recurrentBudgets.Any())
            {
                var newBudgets = recurrentBudgets.Select(rb => new Budget
                {
                    UserId = request.UserId,
                    CategoryId = rb.CategoryId,
                    Amount = rb.Amount,
                    Month = request.Month,
                    Year = request.Year,
                    IsRecurrent = true
                }).ToList();

                _dbContext.Budgets.AddRange(newBudgets);
                await _dbContext.SaveChangesAsync(cancellationToken);

                // Refresh userBudgets list
                userBudgets = await _dbContext.Budgets
                    .Include(b => b.Category)
                    .ThenInclude(c => c.Group)
                    .Where(b => b.UserId == request.UserId && b.Month == request.Month && b.Year == request.Year)
                    .ToListAsync(cancellationToken);
            }
        }

        // 2. Fetch spent amounts (expenses) for these categories in the same period
        // Optimization: only fetching for categories that have budgets
        var categoryIds = userBudgets.Select(b => b.CategoryId).ToList();
        
        var spentItems = await _dbContext.ExpenseItems
            .Include(i => i.Expense)
            .Where(i => i.Expense.UserId == request.UserId && 
                        i.Expense.Date >= startDate && 
                        i.Expense.Date < endDate &&
                        categoryIds.Contains(i.CategoryId))
            .Select(i => new { i.CategoryId, i.ItemAmount })
            .ToListAsync(cancellationToken);

        var spentAmounts = spentItems
            .GroupBy(i => i.CategoryId)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.ItemAmount));

        // 3. Map to DTOs
        var budgets = userBudgets.Select(b => new BudgetDto(
            b.Id,
            b.CategoryId,
            b.Category!.Name,
            b.Category!.Icon,
            b.Category!.Color,
            b.Category?.Group?.Name,
            b.Amount,
            spentAmounts.GetValueOrDefault(b.CategoryId, 0),
            b.IsRecurrent
        )).ToList();

        return new BudgetListDto(budgets, monthlyIncome);
    }
}
