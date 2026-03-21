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
    decimal SpentAmount
);

public record GetBudgetsQuery(
    string UserId,
    int Month,
    int Year
) : IRequest<List<BudgetDto>>;

internal sealed class GetBudgetsQueryHandler : IRequestHandler<GetBudgetsQuery, List<BudgetDto>>
{
    private readonly ApplicationDbContext _dbContext;

    public GetBudgetsQueryHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<BudgetDto>> Handle(GetBudgetsQuery request, CancellationToken cancellationToken)
    {
        // 1. Fetch registered budgets for the month
        var userBudgets = await _dbContext.Budgets
            .Include(b => b.Category)
            .ThenInclude(c => c.Group)
            .Where(b => b.UserId == request.UserId && b.Month == request.Month && b.Year == request.Year)
            .ToListAsync(cancellationToken);

        // 2. Fetch spent amounts (expenses) for these categories in the same period
        // Optimization: only fetching for categories that have budgets
        var categoryIds = userBudgets.Select(b => b.CategoryId).ToList();
        
        var startDate = new DateTime(request.Year, request.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = startDate.AddMonths(1);

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
        return userBudgets.Select(b => new BudgetDto(
            b.Id,
            b.CategoryId,
            b.Category!.Name,
            b.Category!.Icon,
            b.Category!.Color,
            b.Category?.Group?.Name,
            b.Amount,
            spentAmounts.GetValueOrDefault(b.CategoryId, 0)
        )).ToList();
    }
}
