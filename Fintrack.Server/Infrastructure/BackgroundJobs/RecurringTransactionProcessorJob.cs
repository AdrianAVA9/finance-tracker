using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Fintrack.Server.Infrastructure.Data;
using Fintrack.Server.Domain.Enums;
using Fintrack.Server.Domain.Expenses;
using Fintrack.Server.Domain.Incomes;

namespace Fintrack.Server.Infrastructure.BackgroundJobs;

public class RecurringTransactionProcessorJob : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RecurringTransactionProcessorJob> _logger;

    public RecurringTransactionProcessorJob(
        IServiceProvider serviceProvider,
        ILogger<RecurringTransactionProcessorJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RecurringTransactionProcessorJob started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessRecurringIncomesAsync(stoppingToken);
                await ProcessRecurringExpensesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred processing recurring transactions.");
            }

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }

    private async Task ProcessRecurringIncomesAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var today = DateTime.UtcNow.Date;

        var dueIncomes = await dbContext.RecurringIncomes
            .Where(r => r.IsActive && r.NextProcessingDate.Date <= today)
            .ToListAsync(cancellationToken);

        if (!dueIncomes.Any()) return;

        _logger.LogInformation("Found {Count} recurring incomes to process.", dueIncomes.Count);

        foreach (var template in dueIncomes)
        {
            var incomeResult = Income.Create(
                template.UserId,
                template.Source,
                template.Amount,
                template.CategoryId,
                today,
                "Procesado automáticamente desde plantilla recurrente.");

            if (incomeResult.IsFailure)
            {
                _logger.LogWarning(
                    "Skipped recurring income {Id}: {Error}",
                    template.Id,
                    incomeResult.Error.Description);
                continue;
            }

            dbContext.Incomes.Add(incomeResult.Value);

            template.NextProcessingDate = CalculateNextDate(template.NextProcessingDate.Date, template.Frequency, today);
            _logger.LogInformation("Processed recurring income {Id}. Next date: {NextDate:d}", template.Id, template.NextProcessingDate);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task ProcessRecurringExpensesAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var today = DateTime.UtcNow.Date;

        var dueExpenses = await dbContext.RecurringExpenses
            .Where(r => r.IsActive && r.NextProcessingDate.Date <= today)
            .ToListAsync(cancellationToken);

        if (!dueExpenses.Any())
            return;

        _logger.LogInformation("Found {Count} recurring expenses to process.", dueExpenses.Count);

        foreach (var template in dueExpenses)
        {
            var expenseResult = Expense.Create(
                template.UserId,
                template.Amount,
                today,
                template.Merchant,
                status: ExpenseStatus.NeedsReview);

            if (expenseResult.IsFailure)
            {
                _logger.LogWarning(
                    "Skipped recurring expense {Id}: {Error}",
                    template.Id,
                    expenseResult.Error.Description);
                continue;
            }

            var expense = expenseResult.Value;
            var item = ExpenseItem.Create(
                template.CategoryId,
                template.Amount,
                $"Automated recurring charge for {template.Merchant ?? "subscription"}");
            expense.AddItem(item);

            dbContext.Expenses.Add(expense);

            template.NextProcessingDate = CalculateNextDate(template.NextProcessingDate.Date, template.Frequency, today);
            _logger.LogInformation("Processed recurring expense {Id}. Next date: {NextDate:d}", template.Id, template.NextProcessingDate);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static DateTime CalculateNextDate(DateTime currentDate, RecurringFrequency frequency, DateTime today)
    {
        DateTime nextDate = currentDate;

        do
        {
            nextDate = frequency switch
            {
                RecurringFrequency.Daily => nextDate.AddDays(1),
                RecurringFrequency.Weekly => nextDate.AddDays(7),
                RecurringFrequency.BiWeekly => nextDate.AddDays(14),
                RecurringFrequency.Monthly => nextDate.AddMonths(1),
                RecurringFrequency.Quarterly => nextDate.AddMonths(3),
                RecurringFrequency.Yearly => nextDate.AddYears(1),
                _ => nextDate.AddMonths(1)
            };
        } while (nextDate <= today);

        return nextDate;
    }
}
