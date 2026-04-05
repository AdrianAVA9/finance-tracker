using Fintrack.Server.Application.Abstractions.Messaging;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Budgets;
using Fintrack.Server.Domain.Expenses;
using Fintrack.Server.Domain.Incomes;

namespace Fintrack.Server.Application.Budgets.Queries.GetBudgets;

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
) : IQuery<BudgetListDto>;

internal sealed class GetBudgetsQueryHandler : IQueryHandler<GetBudgetsQuery, BudgetListDto>
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly IRecurringIncomeRepository _recurringIncomeRepository;
    private readonly IExpenseRepository _expenseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GetBudgetsQueryHandler(
        IBudgetRepository budgetRepository,
        IRecurringIncomeRepository recurringIncomeRepository,
        IExpenseRepository expenseRepository,
        IUnitOfWork unitOfWork)
    {
        _budgetRepository = budgetRepository;
        _recurringIncomeRepository = recurringIncomeRepository;
        _expenseRepository = expenseRepository;
        _unitOfWork = unitOfWork;
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

        if (!userBudgets.Any())
        {
            var prevDate = startDate.AddMonths(-1);
            var recurrentBudgets = await _budgetRepository.GetRecurrentBudgetsForMonthAsync(
                request.UserId,
                prevDate.Month,
                prevDate.Year,
                cancellationToken);

            if (recurrentBudgets.Any())
            {
                foreach (var rb in recurrentBudgets)
                {
                    var createResult = Budget.Create(
                        request.UserId,
                        rb.CategoryId,
                        rb.Amount,
                        true,
                        request.Month,
                        request.Year);

                    if (createResult.IsSuccess)
                    {
                        _budgetRepository.Add(createResult.Value);
                    }
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                userBudgets = (await _budgetRepository.GetUserBudgetsByMonthWithCategoryAsync(
                    request.UserId,
                    request.Month,
                    request.Year,
                    cancellationToken)).ToList();
            }
        }

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
