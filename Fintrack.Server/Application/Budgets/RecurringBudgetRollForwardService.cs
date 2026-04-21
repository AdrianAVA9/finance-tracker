using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Budgets;
using Microsoft.Extensions.Logging;

namespace Fintrack.Server.Application.Budgets;

/// <summary>
/// Rolls recurrent budgets from the previous calendar month into the current month (first-of-month job).
/// </summary>
public interface IRecurringBudgetRollForwardService
{
    Task ProcessAsync(DateTime utcNow, CancellationToken cancellationToken = default);
}

internal sealed class RecurringBudgetRollForwardService : IRecurringBudgetRollForwardService
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RecurringBudgetRollForwardService> _logger;

    public RecurringBudgetRollForwardService(
        IBudgetRepository budgetRepository,
        IUnitOfWork unitOfWork,
        ILogger<RecurringBudgetRollForwardService> logger)
    {
        _budgetRepository = budgetRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task ProcessAsync(DateTime utcNow, CancellationToken cancellationToken = default)
    {
        var currentYear = utcNow.Year;
        var currentMonth = utcNow.Month;

        var previousMonthDate = utcNow.AddMonths(-1);
        var previousYear = previousMonthDate.Year;
        var previousMonth = previousMonthDate.Month;

        var recurringBudgets = await _budgetRepository.GetRecurrentBudgetsForMonthForAllUsersAsync(
            previousMonth,
            previousYear,
            cancellationToken);

        if (recurringBudgets.Count == 0)
        {
            return;
        }

        _logger.LogInformation("Found {Count} recurring budgets from previous month.", recurringBudgets.Count);

        var addedCount = 0;

        foreach (var previousBudget in recurringBudgets)
        {
            var addResult = await BudgetSlotHelper.TryAddNewBudgetSlotAsync(
                _budgetRepository,
                previousBudget.UserId,
                previousBudget.CategoryId,
                previousBudget.Amount,
                isRecurrent: true,
                currentMonth,
                currentYear,
                cancellationToken);

            if (addResult.IsSuccess)
            {
                addedCount++;
            }
            else if (addResult.Error != BudgetErrors.AlreadyExists)
            {
                _logger.LogWarning(
                    "Skipped recurring budget roll-forward for user {UserId} category {CategoryId}: {Code} {Description}",
                    previousBudget.UserId,
                    previousBudget.CategoryId,
                    addResult.Error.Code,
                    addResult.Error.Description);
            }
        }

        if (addedCount > 0)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation(
                "Successfully registered {AddedCount} recurring budgets for {Year}-{Month}.",
                addedCount,
                currentYear,
                currentMonth);
        }
    }
}
