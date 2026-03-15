using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Fintrack.Server.Data;
using Fintrack.Server.Models;
using Fintrack.Server.Models.Enums;

namespace Fintrack.Server.Infrastructure.BackgroundJobs
{
    public class RecurringExpenseProcessorJob : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RecurringExpenseProcessorJob> _logger;

        public RecurringExpenseProcessorJob(IServiceProvider serviceProvider, ILogger<RecurringExpenseProcessorJob> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("RecurringExpenseProcessorJob started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessRecurringExpensesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred processing recurring expenses.");
                }

                // Runs roughly every 24 hours. For precise daily scheduling, Quartz or Hangfire is recommended.
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }

        private async Task ProcessRecurringExpensesAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var today = DateTime.UtcNow.Date;

            // Find all active templates due today or earlier
            var dueExpenses = await dbContext.RecurringExpenses
                .Where(r => r.IsActive && r.NextProcessingDate.Date <= today)
                .ToListAsync(cancellationToken);

            if (!dueExpenses.Any())
                return;

            _logger.LogInformation($"Found {dueExpenses.Count} recurring expenses to process.");

            foreach (var template in dueExpenses)
            {
                // 1. Create the materialized expense record
                var newExpense = new Expense
                {
                    UserId = template.UserId,
                    TotalAmount = template.Amount,
                    Date = today, // Date of generation
                    Merchant = template.Merchant,
                    // Status is NeedsReview: An automated recurring expense requires manual approval
                    Status = ExpenseStatus.NeedsReview, 
                    Items = new List<ExpenseItem>
                    {
                        new ExpenseItem
                        {
                            CategoryId = template.CategoryId,
                            ItemAmount = template.Amount,
                            Description = $"Automated recurring charge for {template.Merchant ?? "subscription"}"
                        }
                    }
                };

                dbContext.Expenses.Add(newExpense);

                // 2. Update the next processing date safely, even if the job missed a few days
                template.NextProcessingDate = CalculateNextDate(template.NextProcessingDate.Date, template.Frequency, today);
                
                _logger.LogInformation($"Processed recurring expense {template.Id}. Next date: {template.NextProcessingDate:d}");
            }

            // 3. ACID Transaction: all or nothing
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        private DateTime CalculateNextDate(DateTime currentDate, RecurringFrequency frequency, DateTime today)
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
}
