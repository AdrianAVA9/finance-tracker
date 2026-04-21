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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fintrack.Server.Application.Transactions.Queries
{
    public record TransactionDto(
        string Id,
        string Description,
        string CategoryName,
        string? CategoryIcon,
        string? CategoryColor,
        DateTime Date,
        decimal Amount,
        string Type // "Income" or "Expense"
    );

    public record GetTransactionsQuery(
        string UserId, 
        int Year, 
        int Month, 
        string? Type = "All"
    ) : IRequest<List<TransactionDto>>;

    public class GetTransactionsQueryHandler : IRequestHandler<GetTransactionsQuery, List<TransactionDto>>
    {
        private readonly ApplicationDbContext _dbContext;

        public GetTransactionsQueryHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<TransactionDto>> Handle(GetTransactionsQuery request, CancellationToken cancellationToken)
        {
            var monthStart = new DateTime(request.Year, request.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);

            var transactions = new List<TransactionDto>();

            // 1. Fetch Incomes if Type is "All" or "Income"
            if (request.Type == "All" || request.Type == "Income")
            {
                var incomes = await _dbContext.Incomes
                    .Include(i => i.Category)
                    .Where(i => i.UserId == request.UserId && i.Date >= monthStart && i.Date <= monthEnd)
                    .OrderByDescending(i => i.Date)
                    .Select(i => new TransactionDto(
                        "inc_" + i.Id,
                        i.Source,
                        i.Category!.Name,
                        i.Category.Icon,
                        i.Category.Color,
                        i.Date,
                        i.Amount,
                        "Income"
                    ))
                    .ToListAsync(cancellationToken);
                
                transactions.AddRange(incomes);
            }

            // 2. Fetch Expenses if Type is "All" or "Expense"
            if (request.Type == "All" || request.Type == "Expense")
            {
                var expenses = await _dbContext.Expenses
                    .Include(e => e.Items)
                        .ThenInclude(ei => ei.Category)
                    .Where(e => e.UserId == request.UserId && e.Date >= monthStart && e.Date <= monthEnd)
                    .OrderByDescending(e => e.Date)
                    .ToListAsync(cancellationToken);

                var mappedExpenses = expenses.Select(e => new TransactionDto(
                    "exp_" + e.Id,
                    e.Merchant ?? "Gasto",
                    e.Items.FirstOrDefault()?.Category?.Name ?? "General",
                    e.Items.FirstOrDefault()?.Category?.Icon ?? "receipt_long",
                    e.Items.FirstOrDefault()?.Category?.Color,
                    e.Date,
                    -e.TotalAmount,
                    "Expense"
                )).ToList();

                transactions.AddRange(mappedExpenses);
            }

            // 3. Final Ordering
            return transactions.OrderByDescending(t => t.Date).ToList();
        }
    }
}
