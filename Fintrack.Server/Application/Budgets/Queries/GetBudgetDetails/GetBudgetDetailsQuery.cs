using Fintrack.Server.Application.Abstractions.Messaging;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Budgets;
using Fintrack.Server.Domain.Expenses;

namespace Fintrack.Server.Application.Budgets.Queries.GetBudgetDetails;

public record ExpenseItemDto(
    Guid Id,
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
    Guid Id,
    string CategoryName,
    decimal LimitAmount,
    List<MonthlyExpenseSummaryDto> MonthlyHistory
);

public record GetBudgetDetailsQuery(
    Guid BudgetId,
    string UserId,
    int Month,
    int Year
) : IQuery<BudgetDetailsDto>;

internal sealed class GetBudgetDetailsQueryHandler : IQueryHandler<GetBudgetDetailsQuery, BudgetDetailsDto>
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly IExpenseRepository _expenseRepository;

    public GetBudgetDetailsQueryHandler(
        IBudgetRepository budgetRepository,
        IExpenseRepository expenseRepository)
    {
        _budgetRepository = budgetRepository;
        _expenseRepository = expenseRepository;
    }

    public async Task<Result<BudgetDetailsDto>> Handle(
        GetBudgetDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var budget = await _budgetRepository.GetByIdWithCategoryAsync(
            request.BudgetId,
            request.UserId,
            cancellationToken);

        if (budget == null)
        {
            return Result.Failure<BudgetDetailsDto>(BudgetErrors.NotFound);
        }

        var categoryId = budget.CategoryId;

        var endDate = new DateTime(request.Year, request.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(1);
        var startDate = endDate.AddMonths(-12);

        var expenseItems = await _expenseRepository.GetItemsForUserCategoryInDateRangeAsync(
            request.UserId,
            categoryId,
            startDate,
            endDate,
            cancellationToken);

        var monthlySummaries = expenseItems
            .GroupBy(i => new { i.Expense!.Date.Year, i.Expense.Date.Month })
            .Select(g => new MonthlyExpenseSummaryDto(
                g.Key.Month,
                g.Key.Year,
                g.Sum(x => x.ItemAmount),
                g.Select(x => new ExpenseItemDto(
                    x.Id,
                    x.Description ?? "No description",
                    x.ItemAmount,
                    x.Expense!.Date)).ToList()))
            .OrderByDescending(m => m.Year).ThenByDescending(m => m.Month)
            .ToList();

        var filledSummaries = new List<MonthlyExpenseSummaryDto>();
        for (var i = 0; i < 12; i++)
        {
            var currentMonthDate = endDate.AddMonths(-1 - i);
            var existing = monthlySummaries.FirstOrDefault(m =>
                m.Month == currentMonthDate.Month && m.Year == currentMonthDate.Year);
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
                    new List<ExpenseItemDto>()));
            }
        }

        return new BudgetDetailsDto(
            budget.Id,
            budget.Category!.Name,
            budget.Amount,
            filledSummaries);
    }
}
