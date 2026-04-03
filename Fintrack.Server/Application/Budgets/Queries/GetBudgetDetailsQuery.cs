using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Fintrack.Server.Infrastructure.Data;

namespace Fintrack.Server.Application.Budgets.Queries;

public record ExpenseItemDto(
    int Id,
    string Description,
    decimal Amount,
    DateTime Date
);

public record MonthlyExpenseSummaryDto(
    int Month,
    int Year,
    decimal TotalExpense,
    List<ExpenseItemDto> Expenses
);

public record BudgetDetailsDto(
    int Id,
    string CategoryName,
    decimal LimitAmount,
    List<MonthlyExpenseSummaryDto> MonthlyHistory
);

public record GetBudgetDetailsQuery(
    int BudgetId,
    string UserId,
    int Month,
    int Year
) : IRequest<BudgetDetailsDto?>;

internal sealed class GetBudgetDetailsQueryHandler : IRequestHandler<GetBudgetDetailsQuery, BudgetDetailsDto?>
{
    private readonly ApplicationDbContext _dbContext;

    public GetBudgetDetailsQueryHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<BudgetDetailsDto?> Handle(GetBudgetDetailsQuery request, CancellationToken cancellationToken)
    {
        // 1. Fetch the specific budget
        var budget = await _dbContext.Budgets
            .Include(b => b.Category)
            .FirstOrDefaultAsync(b => b.Id == request.BudgetId && b.UserId == request.UserId, cancellationToken);

        if (budget == null) return null;

        var categoryId = budget.CategoryId;
        
        // 2. Calculate the 12-month window looking back from the requested month/year
        var endDate = new DateTime(request.Year, request.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(1);
        var startDate = endDate.AddMonths(-12);

        // 3. Fetch all expense items for this category within the 12-month window
        var expenseItems = await _dbContext.ExpenseItems
            .Include(i => i.Expense)
            .Where(i => i.CategoryId == categoryId && 
                        i.Expense.UserId == request.UserId &&
                        i.Expense.Date >= startDate &&
                        i.Expense.Date < endDate)
            .OrderByDescending(i => i.Expense.Date)
            .Select(i => new {
                i.Id,
                i.Description,
                i.ItemAmount,
                i.Expense.Date
            })
            .ToListAsync(cancellationToken);

        // 4. Group by Month and Year
        var monthlySummaries = expenseItems
            .GroupBy(i => new { i.Date.Year, i.Date.Month })
            .Select(g => new MonthlyExpenseSummaryDto(
                g.Key.Month,
                g.Key.Year,
                g.Sum(x => x.ItemAmount),
                g.Select(x => new ExpenseItemDto(x.Id, x.Description ?? "No description", x.ItemAmount, x.Date)).ToList()
            ))
            // Ensure we show recent first
            .OrderByDescending(m => m.Year).ThenByDescending(m => m.Month)
            .ToList();

        // 5. Fill in missing months with 0 total expense so the 12-month chart is continuous
        var filledSummaries = new List<MonthlyExpenseSummaryDto>();
        for (int i = 0; i < 12; i++)
        {
            var currentMonthDate = endDate.AddMonths(-1 - i);
            var existing = monthlySummaries.FirstOrDefault(m => m.Month == currentMonthDate.Month && m.Year == currentMonthDate.Year);
            if (existing != null)
            {
                filledSummaries.Add(existing);
            }
            else
            {
                filledSummaries.Add(new MonthlyExpenseSummaryDto(
                    currentMonthDate.Month,
                    currentMonthDate.Year,
                    0m,
                    new List<ExpenseItemDto>()
                ));
            }
        }

        return new BudgetDetailsDto(
            budget.Id,
            budget.Category!.Name,
            budget.Amount,
            filledSummaries
        );
    }
}
