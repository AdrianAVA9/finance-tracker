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

namespace Fintrack.Server.Application.Incomes.Commands
{
    public record CreateIncomeCommand(
        string UserId,
        string Source,
        decimal Amount,
        int CategoryId,
        DateTime Date,
        string? Notes,
        bool IsRecurring,
        RecurringFrequency? Frequency = null,
        DateTime? NextDate = null
    ) : IRequest<int>;

    public class CreateIncomeCommandHandler : IRequestHandler<CreateIncomeCommand, int>
    {
        private readonly ApplicationDbContext _dbContext;

        public CreateIncomeCommandHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> Handle(CreateIncomeCommand request, CancellationToken cancellationToken)
        {
            // 1. Validation
            if (request.Amount <= 0)
            {
                throw new ArgumentException("El monto del ingreso debe ser mayor a cero.");
            }

            if (request.Date > DateTime.UtcNow.AddYears(1))
            {
                throw new ArgumentException("La fecha del ingreso no puede ser superior a un año en el futuro.");
            }

            // 2. Create actual Income
            var income = new Income
            {
                UserId = request.UserId,
                Source = request.Source,
                Amount = request.Amount,
                CategoryId = request.CategoryId,
                Date = request.Date,
                Notes = request.Notes
            };

            _dbContext.Incomes.Add(income);

            // 3. Handle Recurrence Template
            if (request.IsRecurring && request.Frequency.HasValue)
            {
                var nextDate = request.NextDate ?? CalculateInitialNextDate(request.Date, request.Frequency.Value);
                
                var recurringTemplate = new RecurringIncome
                {
                    UserId = request.UserId,
                    Source = request.Source,
                    Amount = request.Amount,
                    CategoryId = request.CategoryId,
                    Frequency = request.Frequency.Value,
                    StartDate = request.Date,
                    NextProcessingDate = nextDate,
                    IsActive = true
                };

                _dbContext.RecurringIncomes.Add(recurringTemplate);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            return income.Id;
        }

        private DateTime CalculateInitialNextDate(DateTime startDate, RecurringFrequency frequency)
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
