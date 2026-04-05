using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using Fintrack.Server.Infrastructure.Data;
using Fintrack.Server.Infrastructure.BackgroundJobs;
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
using MediatR;
using NSubstitute;

namespace Fintrack.Tests.Infrastructure.BackgroundJobs
{
    public class RecurringTransactionProcessorJobTests
    {
        private ApplicationDbContext GetInMemoryContext()
        {
            var publisher = Substitute.For<IPublisher>();
            publisher.Publish(Arg.Any<INotification>(), Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask);

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options, publisher);
        }
 
        [Fact]
        public void Should_CalculateNextProcessingDate_Correctly_SupportingEdgeCases()
        {
            // Arrange
            var serviceProvider = Substitute.For<IServiceProvider>();
            var logger = Substitute.For<ILogger<RecurringTransactionProcessorJob>>();
            var job = new RecurringTransactionProcessorJob(serviceProvider, logger);
 
            // Reflection to access the private CalculateNextDate method
            var methodInfo = typeof(RecurringTransactionProcessorJob).GetMethod("CalculateNextDate", BindingFlags.NonPublic | BindingFlags.Instance);

            // Edge Case 1: Monthly from Jan 31st. Should be Feb 28th (or 29th on leap year).
            var jan31 = new DateTime(2025, 1, 31);
            var today = new DateTime(2025, 1, 31);
            var nextDate1 = (DateTime)methodInfo.Invoke(job, new object[] { jan31, RecurringFrequency.Monthly, today });
            Assert.Equal(new DateTime(2025, 2, 28), nextDate1.Date);

            // Edge Case 2: Past due dates (catch-up mechanism)
            // If the job was paused and a weekly expense from 10 days ago needs processing today
            var pastWeekly = new DateTime(2025, 1, 10);
            var currentToday = new DateTime(2025, 1, 20);
            var nextDateCatchup = (DateTime)methodInfo.Invoke(job, new object[] { pastWeekly, RecurringFrequency.Weekly, currentToday });
            
            // Should increment 7 days iteratively until strictly > today:
            // 10 + 7 = 17 (<= 20) -> loop
            // 17 + 7 = 24 (> 20) -> break
            Assert.Equal(new DateTime(2025, 1, 24), nextDateCatchup.Date);
        }

        [Fact]
        public async Task Should_ProcessDueExpenses_And_GenerateNewExpenseRecords()
        {
            // Arrange
            using var context = GetInMemoryContext();
            
            // Insert a due template
            var template = new RecurringExpense
            {
                UserId = "user-1",
                Merchant = "Netflix",
                Amount = 15.99m,
                CategoryId = 1,
                Frequency = RecurringFrequency.Monthly,
                StartDate = DateTime.UtcNow.AddMonths(-1),
                NextProcessingDate = DateTime.UtcNow.AddDays(-1), // Due yesterday
                IsActive = true
            };
            context.RecurringExpenses.Add(template);
            await context.SaveChangesAsync();

            var serviceScope = Substitute.For<IServiceScope>();
            var serviceScopeFactory = Substitute.For<IServiceScopeFactory>();
            
            serviceScope.ServiceProvider.GetService(typeof(ApplicationDbContext)).Returns(context);
            serviceScopeFactory.CreateScope().Returns(serviceScope);

            var serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService(typeof(IServiceScopeFactory)).Returns(serviceScopeFactory);

            var logger = Substitute.For<ILogger<RecurringTransactionProcessorJob>>();
            var job = new RecurringTransactionProcessorJob(serviceProvider, logger);
 
            // Act - invoking the private logic directly to avoid the infinite while loop in ExecuteAsync
            var methodInfo = typeof(RecurringTransactionProcessorJob).GetMethod("ProcessRecurringExpensesAsync", BindingFlags.NonPublic | BindingFlags.Instance);
            var task = (Task)methodInfo.Invoke(job, new object[] { CancellationToken.None });
            await task;

            // Assert
            var newExpense = await context.Expenses.Include(e => e.Items).SingleOrDefaultAsync();
            Assert.NotNull(newExpense);
            Assert.Equal("Netflix", newExpense.Merchant);
            Assert.Equal(ExpenseStatus.NeedsReview, newExpense.Status); // automated = NeedsReview
            Assert.Single(newExpense.Items);
            Assert.Equal(15.99m, newExpense.Items.First().ItemAmount);

            // Template date moved forward
            var updatedTemplate = await context.RecurringExpenses.SingleAsync();
            Assert.True(updatedTemplate.NextProcessingDate > DateTime.UtcNow.Date);
        }
    }
}
