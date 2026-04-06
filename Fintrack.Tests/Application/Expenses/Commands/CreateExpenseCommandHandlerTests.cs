using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Fintrack.Server.Infrastructure.Data;
using Fintrack.Server.Application.Expenses.Commands;
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

namespace Fintrack.Tests.Application.Expenses.Commands
{
    public class CreateExpenseCommandHandlerTests
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
        public async Task Should_FailValidation_When_ItemSum_DiffersFrom_TotalAmount()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var handler = new CreateExpenseCommandHandler(context);

            var command = new CreateExpenseCommand(
                UserId: "user-1",
                TotalAmount: 100.00m,
                Date: DateTime.UtcNow,
                Merchant: "Store",
                InvoiceNumber: null,
                InvoiceImageUrl: null,
                IsRecurring: false,
                Frequency: null,
                Items: new List<ExpenseItemDto>
                {
                    new ExpenseItemDto(Guid.NewGuid(), 50.00m, "Item 1"),
                    new ExpenseItemDto(Guid.NewGuid(), 49.99m, "Item 2") // Sum is 99.99, not 100.00
                }
            );

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ValidationException>(() => 
                handler.Handle(command, CancellationToken.None));

            Assert.Contains("Strict Math Validation Failed", ex.Message);
            Assert.Empty(context.Expenses);
        }

        [Fact]
        public async Task Should_SaveSuccessfully_When_ValidationPasses_WithAtomicTransaction()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var handler = new CreateExpenseCommandHandler(context);

            // Math is perfect: 50.00 + 50.00 = 100.00
            var command = new CreateExpenseCommand(
                UserId: "user-1",
                TotalAmount: 100.00m,
                Date: DateTime.UtcNow,
                Merchant: "Target",
                InvoiceNumber: "INV-123",
                InvoiceImageUrl: null,
                IsRecurring: true,
                Frequency: RecurringFrequency.Monthly,
                Items: new List<ExpenseItemDto>
                {
                    new ExpenseItemDto(Guid.NewGuid(), 50.00m, "Item 1"),
                    new ExpenseItemDto(Guid.NewGuid(), 50.00m, "Item 2")
                }
            );

            // Act
            var expenseId = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(expenseId > 0);
            
            var savedExpense = await context.Expenses
                .Include(e => e.Items)
                .FirstOrDefaultAsync(e => e.Id == expenseId);

            Assert.NotNull(savedExpense);
            Assert.Equal(100.00m, savedExpense.TotalAmount);
            Assert.Equal(2, savedExpense.Items.Count);
            
            // Check atomic recurring save
            var savedRecurring = await context.RecurringExpenses.FirstOrDefaultAsync();
            Assert.NotNull(savedRecurring);
            Assert.Equal(100.00m, savedRecurring.Amount);
            Assert.Equal(RecurringFrequency.Monthly, savedRecurring.Frequency);
        }
    }
}
