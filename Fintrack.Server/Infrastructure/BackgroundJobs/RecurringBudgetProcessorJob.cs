using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fintrack.Server.Application.Budgets;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Budgets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Fintrack.Server.Infrastructure.BackgroundJobs;

public class RecurringBudgetProcessorJob : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RecurringBudgetProcessorJob> _logger;

    public RecurringBudgetProcessorJob(IServiceProvider serviceProvider, ILogger<RecurringBudgetProcessorJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RecurringBudgetProcessorJob started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (DateTime.UtcNow.Day == 1)
                {
                    await ProcessRecurringBudgetsAsync(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred processing recurring budgets.");
            }

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }

    private async Task ProcessRecurringBudgetsAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var budgetRepository = scope.ServiceProvider.GetRequiredService<IBudgetRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var now = DateTime.UtcNow;
        int currentYear = now.Year;
        int currentMonth = now.Month;

        var previousMonthDate = now.AddMonths(-1);
        int previousYear = previousMonthDate.Year;
        int previousMonth = previousMonthDate.Month;

        var recurringBudgets = await budgetRepository.GetRecurrentBudgetsForMonthForAllUsersAsync(
            previousMonth,
            previousYear,
            cancellationToken);

        if (!recurringBudgets.Any())
        {
            return;
        }

        _logger.LogInformation("Found {Count} recurring budgets from previous month.", recurringBudgets.Count);

        var addedCount = 0;

        foreach (var previousBudget in recurringBudgets)
        {
            var addResult = await BudgetSlotHelper.TryAddNewBudgetSlotAsync(
                budgetRepository,
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
            await unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation(
                "Successfully registered {AddedCount} recurring budgets for {Year}-{Month}.",
                addedCount,
                currentYear,
                currentMonth);
        }
    }
}
