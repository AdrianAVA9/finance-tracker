using Fintrack.Server.Application.Abstractions.Messaging;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Budgets;
using Fintrack.Server.Domain.Expenses;
using Fintrack.Server.Domain.Incomes;

namespace Fintrack.Server.Application.Budgets.Queries.GetBudgets;

public record BudgetDto(
    Guid Id,
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
) : IQuery<BudgetListDto>;

internal sealed class GetBudgetsQueryHandler : IQueryHandler<GetBudgetsQuery, BudgetListDto>
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly IRecurringIncomeRepository _recurringIncomeRepository;
    private readonly IExpenseRepository _expenseRepository;

    public GetBudgetsQueryHandler(
        IBudgetRepository budgetRepository,
        IRecurringIncomeRepository recurringIncomeRepository,
        IExpenseRepository expenseRepository)
    {
        _budgetRepository = budgetRepository;
        _recurringIncomeRepository = recurringIncomeRepository;
        _expenseRepository = expenseRepository;
    }

    public async Task<Result<BudgetListDto>> Handle(GetBudgetsQuery request, CancellationToken cancellationToken)
    {
        var startDate = new DateTime(request.Year, request.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = startDate.AddMonths(1);

        var monthlyIncome = await _recurringIncomeRepository.SumActiveAmountForUserBeforeAsync(
            request.UserId,
            endDate,
            cancellationToken);

        var userBudgets = (await _budgetRepository.GetUserBudgetsByMonthWithCategoryAsync(
            request.UserId,
            request.Month,
            request.Year,
            cancellationToken)).ToList();

        var categoryIds = userBudgets.Select(b => b.CategoryId).ToList();

        var spentAmounts = await _expenseRepository.SumItemAmountsByCategoryAsync(
            request.UserId,
            startDate,
            endDate,
            categoryIds,
            cancellationToken);

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
