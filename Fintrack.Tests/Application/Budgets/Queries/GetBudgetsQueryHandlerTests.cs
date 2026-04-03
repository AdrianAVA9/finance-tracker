using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Fintrack.Server.Infrastructure.Data;
using Fintrack.Server.Application.Budgets.Queries;
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

namespace Fintrack.Tests.Application.Budgets.Queries
{
    public class GetBudgetsQueryHandlerTests
    {
        private ApplicationDbContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Should_Return_Budgets_With_Correct_SpentAmounts()
        {
            // Arrange
            using var context = GetInMemoryContext();
            
            var userId = "test-user";
            var month = 3;
            var year = 2024;

            // 1. Seed Categories
            var group = new ExpenseCategoryGroup { Id = 1, Name = "Vivienda" };
            var category = new ExpenseCategory 
            { 
                Id = 1, 
                Name = "Alquiler", 
                GroupId = 1, 
                Group = group,
                Color = "#FF0000",
                Icon = "home"
            };
            context.ExpenseCategoryGroups.Add(group);
            context.ExpenseCategories.Add(category);

            // 2. Seed Budget
            var budget = new Budget
            {
                Id = 1,
                UserId = userId,
                CategoryId = 1,
                Amount = 1000.00m,
                Month = month,
                Year = year
            };
            context.Budgets.Add(budget);

            // 3. Seed Recurring Income (Expected Income)
            var recurringIncome = new RecurringIncome
            {
                UserId = userId,
                Source = "Salario",
                Amount = 5000.00m,
                CategoryId = 1, // Assume a category exists for test
                IsActive = true,
                StartDate = new DateTime(2024, 1, 1),
                NextProcessingDate = new DateTime(2024, 3, 1),
                Frequency = Fintrack.Server.Domain.Enums.RecurringFrequency.Monthly
            };
            context.RecurringIncomes.Add(recurringIncome);

            // 4. Seed Expenses for this month
            var expense = new Expense
            {
                Id = 1,
                UserId = userId,
                Date = new DateTime(year, month, 15),
                Merchant = "Rent Corp",
                TotalAmount = 800.00m
            };
            context.Expenses.Add(expense);

            var expenseItem = new ExpenseItem
            {
                Id = 1,
                ExpenseId = 1,
                CategoryId = 1,
                ItemAmount = 800.00m,
                Description = "Marzo Rent"
            };
            context.ExpenseItems.Add(expenseItem);

            await context.SaveChangesAsync();

            var handler = new GetBudgetsQueryHandler(context);
            var query = new GetBudgetsQuery(userId, month, year);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Single(result.Budgets);
            var dto = result.Budgets.First();
            Assert.Equal(1, dto.CategoryId);
            Assert.Equal(1000.00m, dto.LimitAmount);
            Assert.Equal(800.00m, dto.SpentAmount);
            Assert.Equal(5000.00m, result.MonthlyIncome);
        }

        [Fact]
        public async Task Should_Return_Empty_List_When_No_Budgets_Exist()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var handler = new GetBudgetsQueryHandler(context);
            var query = new GetBudgetsQuery("user-1", 1, 2024);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Empty(result.Budgets);
            Assert.Equal(0, result.MonthlyIncome);
        }
    }
}
