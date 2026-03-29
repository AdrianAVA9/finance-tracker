using MediatR;
using Fintrack.Server.Data;
using Fintrack.Server.Models;
using Microsoft.EntityFrameworkCore;
using Fintrack.Server.Models.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Fintrack.Server.Application.Expenses.Queries
{
    public record ExpenseItemDetailsDto(int Id, int CategoryId, string CategoryName, decimal ItemAmount, string? Description);
    
    public record ExpenseDetailsDto(
        int Id,
        string? Merchant,
        decimal TotalAmount,
        DateTime Date,
        bool IsRecurring,
        RecurringFrequency? Frequency,
        DateTime? NextDate,
        List<ExpenseItemDetailsDto> Items
    );

    public record GetExpenseByIdQuery(int Id, string UserId) : IRequest<ExpenseDetailsDto?>;

    public class GetExpenseByIdQueryHandler : IRequestHandler<GetExpenseByIdQuery, ExpenseDetailsDto?>
    {
        private readonly ApplicationDbContext _dbContext;

        public GetExpenseByIdQueryHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ExpenseDetailsDto?> Handle(GetExpenseByIdQuery request, CancellationToken cancellationToken)
        {
            var expense = await _dbContext.Expenses
                .Include(e => e.Items)
                .ThenInclude(i => i.Category)
                .FirstOrDefaultAsync(e => e.Id == request.Id && e.UserId == request.UserId, cancellationToken);

            if (expense == null) return null;

            // Detect recurrence
            var recurring = await _dbContext.RecurringExpenses
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.UserId == request.UserId && r.Merchant == expense.Merchant && r.Amount == expense.TotalAmount && r.IsActive, cancellationToken);

            return new ExpenseDetailsDto(
                expense.Id,
                expense.Merchant,
                expense.TotalAmount,
                expense.Date,
                recurring != null,
                recurring?.Frequency,
                recurring?.NextProcessingDate,
                expense.Items.Select(i => new ExpenseItemDetailsDto(
                    i.Id,
                    i.CategoryId,
                    i.Category?.Name ?? "Unknown",
                    i.ItemAmount,
                    i.Description
                )).ToList()
            );
        }
    }
}
