using MediatR;
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
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Fintrack.Server.Application.Expenses.Commands
{
    public record UpdateExpenseItemDto(int? Id, int CategoryId, decimal ItemAmount, string? Description);

    public record UpdateExpenseCommand(
        int Id,
        string UserId,
        decimal TotalAmount,
        DateTime Date,
        string? Merchant,
        string? InvoiceNumber,
        string? InvoiceImageUrl,
        bool IsRecurring,
        RecurringFrequency? Frequency,
        List<UpdateExpenseItemDto> Items
    ) : IRequest<bool>;

    public class UpdateExpenseCommandHandler : IRequestHandler<UpdateExpenseCommand, bool>
    {
        private readonly ApplicationDbContext _dbContext;

        public UpdateExpenseCommandHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(UpdateExpenseCommand request, CancellationToken cancellationToken)
        {
            var expense = await _dbContext.Expenses
                .Include(e => e.Items)
                .FirstOrDefaultAsync(e => e.Id == request.Id && e.UserId == request.UserId, cancellationToken);

            if (expense == null) return false;

            // Strict Mathematical Validation
            decimal itemsSum = request.Items?.Sum(i => i.ItemAmount) ?? 0;
            if (itemsSum != request.TotalAmount)
            {
                throw new ValidationException($"Strict Math Validation Failed: The sum of items ({itemsSum}) does not equal the Total Amount ({request.TotalAmount}).");
            }

            // Update basic fields
            expense.Merchant = request.Merchant;
            expense.TotalAmount = request.TotalAmount;
            expense.Date = request.Date;
            expense.InvoiceNumber = request.InvoiceNumber;
            expense.InvoiceImageUrl = request.InvoiceImageUrl;
            expense.UpdatedAt = DateTimeOffset.UtcNow;

            // Sync Items (Wipe and Re-add for simplicity in this specific domain model)
            _dbContext.ExpenseItems.RemoveRange(expense.Items);
            expense.Items = request.Items.Select(i => new ExpenseItem
            {
                CategoryId = i.CategoryId,
                ItemAmount = i.ItemAmount,
                Description = i.Description,
                ExpenseId = expense.Id
            }).ToList();

            // Sync Recurring Template
            var existingRecurring = await _dbContext.RecurringExpenses
                .FirstOrDefaultAsync(r => r.UserId == request.UserId && r.Merchant == expense.Merchant && r.Amount == expense.TotalAmount && r.IsActive, cancellationToken);

            if (request.IsRecurring)
            {
                if (existingRecurring != null)
                {
                    existingRecurring.Merchant = request.Merchant;
                    existingRecurring.Amount = request.TotalAmount;
                    existingRecurring.CategoryId = request.Items.First().CategoryId;
                    existingRecurring.Frequency = request.Frequency ?? RecurringFrequency.Monthly;
                    existingRecurring.UpdatedAt = DateTimeOffset.UtcNow;
                }
                else
                {
                    var recurringTemplate = new RecurringExpense
                    {
                        UserId = request.UserId,
                        Merchant = request.Merchant,
                        Amount = request.TotalAmount,
                        CategoryId = request.Items.First().CategoryId,
                        Frequency = request.Frequency ?? RecurringFrequency.Monthly,
                        StartDate = request.Date,
                        NextProcessingDate = CalculateNextDate(request.Date, request.Frequency ?? RecurringFrequency.Monthly),
                        IsActive = true
                    };
                    _dbContext.RecurringExpenses.Add(recurringTemplate);
                }
            }
            else if (existingRecurring != null)
            {
                existingRecurring.IsActive = false;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
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
