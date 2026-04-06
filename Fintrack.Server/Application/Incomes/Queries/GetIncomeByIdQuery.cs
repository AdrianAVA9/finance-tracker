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
using Microsoft.EntityFrameworkCore;

namespace Fintrack.Server.Application.Incomes.Queries
{
    public record IncomeDetailsDto(
        int Id,
        string Source,
        decimal Amount,
        Guid CategoryId,
        DateTime Date,
        string? Notes,
        bool IsRecurring,
        RecurringFrequency? Frequency,
        DateTime? NextDate
    );

    public record GetIncomeByIdQuery(int Id, string UserId) : IRequest<IncomeDetailsDto?>;

    public class GetIncomeByIdQueryHandler : IRequestHandler<GetIncomeByIdQuery, IncomeDetailsDto?>
    {
        private readonly ApplicationDbContext _dbContext;

        public GetIncomeByIdQueryHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IncomeDetailsDto?> Handle(GetIncomeByIdQuery request, CancellationToken cancellationToken)
        {
            var income = await _dbContext.Incomes
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == request.Id && i.UserId == request.UserId, cancellationToken);

            if (income == null) return null;

            // Check if there's a corresponding active recurring template
            // For now, we'll try to match by UserId and Source/Amount/Category to see if it was created as recurring
            var recurring = await _dbContext.RecurringIncomes
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.UserId == request.UserId && r.Source == income.Source && r.CategoryId == income.CategoryId && r.Amount == income.Amount && r.IsActive, cancellationToken);

            return new IncomeDetailsDto(
                income.Id,
                income.Source,
                income.Amount,
                income.CategoryId,
                income.Date,
                income.Notes,
                recurring != null,
                recurring?.Frequency,
                recurring?.NextProcessingDate
            );
        }
    }
}
