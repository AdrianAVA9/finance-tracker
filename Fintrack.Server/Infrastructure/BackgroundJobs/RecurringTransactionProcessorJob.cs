using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Fintrack.Server.Infrastructure.Data;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Budgets;
using Fintrack.Server.Domain.Enums;
using Fintrack.Server.Domain.Exceptions;
using Fintrack.Server.Domain.ExpenseCategories;
using Fintrack.Server.Domain.Expenses;
using Fintrack.Server.Domain.Incomes;
using Fintrack.Server.Domain.Invoices;
using Fintrack.Server.Domain.SavingsGoals;
using Fintrack.Server.Domain.Users;
using Fintrack.Server.Domain.Enums;

namespace Fintrack.Server.Infrastructure.BackgroundJobs
{
    public class RecurringTransactionProcessorJob : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RecurringTransactionProcessorJob> _logger;

        public RecurringTransactionProcessorJob(IServiceProvider serviceProvider, ILogger<RecurringTransactionProcessorJob> logger)
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

            _logger.LogInformation($"Found {dueIncomes.Count} recurring incomes to process.");

            foreach (var template in dueIncomes)
            {
                var newIncome = new Income
                {
                    UserId = template.UserId,
                    Amount = template.Amount,
                    Source = template.Source,
                    CategoryId = template.CategoryId,
                    Date = today,
                    Notes = $"Procesado automáticamente desde plantilla recurrente."
                };

                dbContext.Incomes.Add(newIncome);

                template.NextProcessingDate = CalculateNextDate(template.NextProcessingDate.Date, template.Frequency, today);
                _logger.LogInformation($"Processed recurring income {template.Id}. Next date: {template.NextProcessingDate:d}");
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

            _logger.LogInformation($"Found {dueExpenses.Count} recurring expenses to process.");

            foreach (var template in dueExpenses)
            {
                var newExpense = new Expense
                {
                    UserId = template.UserId,
                    TotalAmount = template.Amount,
                    Date = today,
                    Merchant = template.Merchant,
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

                template.NextProcessingDate = CalculateNextDate(template.NextProcessingDate.Date, template.Frequency, today);
                _logger.LogInformation($"Processed recurring expense {template.Id}. Next date: {template.NextProcessingDate:d}");
            }

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
