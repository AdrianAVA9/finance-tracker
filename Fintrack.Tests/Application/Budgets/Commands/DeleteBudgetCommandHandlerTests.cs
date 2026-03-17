using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Fintrack.Server.Data;
using Fintrack.Server.Application.Budgets.Commands;
using Fintrack.Server.Models;

namespace Fintrack.Tests.Application.Budgets.Commands
{
    public class DeleteBudgetCommandHandlerTests
    {
        private ApplicationDbContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Should_Delete_Budget_When_Exists_And_Belongs_To_User()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var userId = "user-1";
            var budgetId = 1;

            context.Budgets.Add(new Budget
            {
                Id = budgetId,
                UserId = userId,
                CategoryId = 1,
                Amount = 100.00m,
                Month = 1,
                Year = 2024
            });
            await context.SaveChangesAsync();

            var handler = new DeleteBudgetCommandHandler(context);
            var command = new DeleteBudgetCommand(budgetId, userId);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            var budget = await context.Budgets.FindAsync(budgetId);
            Assert.Null(budget);
        }

        [Fact]
        public async Task Should_Not_Delete_Budget_When_Belongs_To_Different_User()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var ownerId = "owner";
            var attackerId = "attacker";
            var budgetId = 1;

            context.Budgets.Add(new Budget
            {
                Id = budgetId,
                UserId = ownerId,
                CategoryId = 1,
                Amount = 100.00m,
                Month = 1,
                Year = 2024
            });
            await context.SaveChangesAsync();

            var handler = new DeleteBudgetCommandHandler(context);
            var command = new DeleteBudgetCommand(budgetId, attackerId);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            var budget = await context.Budgets.FindAsync(budgetId);
            Assert.NotNull(budget);
            Assert.Equal(ownerId, budget.UserId);
        }

        [Fact]
        public async Task Should_Not_Throw_When_Budget_Does_Not_Exist()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var handler = new DeleteBudgetCommandHandler(context);
            var command = new DeleteBudgetCommand(999, "user-1");

            // Act & Assert
            var exception = await Record.ExceptionAsync(() => 
                handler.Handle(command, CancellationToken.None));
            
            Assert.Null(exception);
        }
    }
}
