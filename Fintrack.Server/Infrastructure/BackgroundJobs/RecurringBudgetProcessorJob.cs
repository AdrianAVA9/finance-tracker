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

namespace Fintrack.Server.Infrastructure.BackgroundJobs
{
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
                    // Run the job logic only on the first day of the month
                    if (DateTime.UtcNow.Day == 1)
                    {
                        await ProcessRecurringBudgetsAsync(stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred processing recurring budgets.");
                }

                // Check every day
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }

        private async Task ProcessRecurringBudgetsAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var now = DateTime.UtcNow;
            int currentYear = now.Year;
            int currentMonth = now.Month;

            var previousMonthDate = now.AddMonths(-1);
            int previousYear = previousMonthDate.Year;
            int previousMonth = previousMonthDate.Month;

            // Find all recurrent budgets from the previous month
            var recurringBudgets = await dbContext.Budgets
                .Where(b => b.Year == previousYear && b.Month == previousMonth && b.IsRecurrent)
                .ToListAsync(cancellationToken);

            if (!recurringBudgets.Any()) return;

            _logger.LogInformation($"Found {recurringBudgets.Count} recurring budgets from previous month.");

            int addedCount = 0;

            foreach (var previousBudget in recurringBudgets)
            {
                // Check if a budget for the current month and category already exists for this user
                var exists = await dbContext.Budgets.AnyAsync(b => 
                    b.UserId == previousBudget.UserId && 
                    b.CategoryId == previousBudget.CategoryId && 
                    b.Year == currentYear && 
                    b.Month == currentMonth, 
                    cancellationToken);

                if (!exists)
                {
                    var newBudget = new Budget
                    {
                        UserId = previousBudget.UserId,
                        CategoryId = previousBudget.CategoryId,
                        Amount = previousBudget.Amount,
                        IsRecurrent = true,
                        Month = currentMonth,
                        Year = currentYear
                    };

                    dbContext.Budgets.Add(newBudget);
                    addedCount++;
                }
            }

            if (addedCount > 0)
            {
                await dbContext.SaveChangesAsync(cancellationToken);
                _logger.LogInformation($"Successfully registered {addedCount} recurring budgets for {currentYear}-{currentMonth}.");
            }
        }
    }
}
