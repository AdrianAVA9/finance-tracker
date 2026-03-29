using MediatR;
using Fintrack.Server.Data;
using Fintrack.Server.Models;
using Fintrack.Server.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Fintrack.Server.Application.Incomes.Commands
{
    public record UpdateIncomeCommand(
        int Id,
        string UserId,
        string Source,
        decimal Amount,
        int CategoryId,
        DateTime Date,
        string? Notes,
        bool IsRecurring,
        RecurringFrequency? Frequency,
        DateTime? NextDate
    ) : IRequest<bool>;

    public class UpdateIncomeCommandHandler : IRequestHandler<UpdateIncomeCommand, bool>
    {
        private readonly ApplicationDbContext _dbContext;

        public UpdateIncomeCommandHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(UpdateIncomeCommand request, CancellationToken cancellationToken)
        {
            var income = await _dbContext.Incomes
                .FirstOrDefaultAsync(i => i.Id == request.Id && i.UserId == request.UserId, cancellationToken);

            if (income == null) return false;

            // Update basic fields
            income.Source = request.Source;
            income.Amount = request.Amount;
            income.CategoryId = request.CategoryId;
            income.Date = request.Date;
            income.Notes = request.Notes;
            income.UpdatedAt = DateTimeOffset.UtcNow;

            // Handle Recurrence Sync
            // 1. Check if there was an existing template
            var existingRecurring = await _dbContext.RecurringIncomes
                .FirstOrDefaultAsync(r => r.UserId == request.UserId && r.Source == income.Source && r.CategoryId == income.CategoryId && r.Amount == income.Amount && r.IsActive, cancellationToken);

            if (request.IsRecurring)
            {
                if (existingRecurring != null)
                {
                    // Update existing
                    existingRecurring.Source = request.Source;
                    existingRecurring.Amount = request.Amount;
                    existingRecurring.CategoryId = request.CategoryId;
                    existingRecurring.Frequency = request.Frequency ?? RecurringFrequency.Monthly;
                    if (request.NextDate.HasValue) existingRecurring.NextProcessingDate = request.NextDate.Value;
                }
                else
                {
                    // Create new
                    var recurringTemplate = new RecurringIncome
                    {
                        UserId = request.UserId,
                        Source = request.Source,
                        Amount = request.Amount,
                        CategoryId = request.CategoryId,
                        Frequency = request.Frequency ?? RecurringFrequency.Monthly,
                        StartDate = request.Date,
                        NextProcessingDate = request.NextDate ?? CalculateNextDate(request.Date, request.Frequency ?? RecurringFrequency.Monthly),
                        IsActive = true
                    };
                    _dbContext.RecurringIncomes.Add(recurringTemplate);
                }
            }
            else if (existingRecurring != null)
            {
                // Deactivate if it was recurring but no longer is
                existingRecurring.IsActive = false;
                // Since this is a "Hard delete" project preference for transactions, but templates are stateful, we deactivate.
                // However, the user said "hard-delete" for transactions. Let's see if they meant templates too.
                // We'll stick to deactivating for safety unless asked.
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
