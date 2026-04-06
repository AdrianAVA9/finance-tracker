using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
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
using Fintrack.Server.Infrastructure.Data;

namespace Fintrack.Server.Application.Expenses.Commands
{
    public record ExpenseItemDto(Guid CategoryId, decimal ItemAmount, string? Description);

    public record CreateExpenseCommand(
        string UserId,
        decimal TotalAmount,
        DateTime Date,
        string? Merchant,
        string? InvoiceNumber,
        string? InvoiceImageUrl,
        bool IsRecurring,
        RecurringFrequency? Frequency,
        List<ExpenseItemDto> Items
    ) : IRequest<int>;

    internal sealed class CreateExpenseCommandHandler : IRequestHandler<CreateExpenseCommand, int>
    {
        private readonly ApplicationDbContext _dbContext;

        public CreateExpenseCommandHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
        {
            // Strict Mathematical Validation
            decimal itemsSum = request.Items?.Sum(i => i.ItemAmount) ?? 0;
            if (itemsSum != request.TotalAmount)
            {
                throw new ValidationException($"Strict Math Validation Failed: The sum of items ({itemsSum}) does not equal the Total Amount ({request.TotalAmount}).");
            }

            // If empty items, fail
            if (request.Items == null || !request.Items.Any())
            {
                throw new ValidationException("Expense must contain at least one item.");
            }

            var expense = new Expense
            {
                UserId = request.UserId,
                TotalAmount = request.TotalAmount,
                Date = request.Date,
                Merchant = request.Merchant,
                InvoiceNumber = request.InvoiceNumber,
                InvoiceImageUrl = request.InvoiceImageUrl,
                Status = ExpenseStatus.Completed,
                Items = request.Items.Select(i => new ExpenseItem
                {
                    CategoryId = i.CategoryId,
                    ItemAmount = i.ItemAmount,
                    Description = i.Description
                }).ToList()
            };

            _dbContext.Expenses.Add(expense);

            if (request.IsRecurring && request.Frequency.HasValue)
            {
                var recurringExpense = new RecurringExpense
                {
                    UserId = request.UserId,
                    Merchant = request.Merchant,
                    Amount = request.TotalAmount,
                    CategoryId = request.Items.First().CategoryId, // using first category as primary
                    Frequency = request.Frequency.Value,
                    StartDate = request.Date,
                    NextProcessingDate = CalculateNextDate(request.Date, request.Frequency.Value),
                    IsActive = true
                };
                
                _dbContext.RecurringExpenses.Add(recurringExpense);
            }

            // ACID Transaction: Add expense, items, and recurring template as a single transaction Unit of Work natively supported by SaveChangesAsync
            await _dbContext.SaveChangesAsync(cancellationToken);

            return expense.Id;
        }

        private DateTime CalculateNextDate(DateTime startDate, RecurringFrequency frequency)
        {
            return frequency switch
            {
                RecurringFrequency.Daily => startDate.AddDays(1),
                RecurringFrequency.Weekly => startDate.AddDays(7),
                RecurringFrequency.BiWeekly => startDate.AddDays(14),
                RecurringFrequency.Monthly => startDate.AddMonths(1),
                RecurringFrequency.Quarterly => startDate.AddMonths(3),
                RecurringFrequency.Yearly => startDate.AddYears(1),
                _ => startDate.AddMonths(1)
            };
        }
    }
}
