using MediatR;
using Fintrack.Server.Data;
using Fintrack.Server.Models;
using Microsoft.EntityFrameworkCore;
using Fintrack.Server.Models.Enums;

namespace Fintrack.Server.Application.Incomes.Queries
{
    public record IncomeDetailsDto(
        int Id,
        string Source,
        decimal Amount,
        int CategoryId,
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
