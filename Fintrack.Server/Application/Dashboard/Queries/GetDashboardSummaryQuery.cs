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
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fintrack.Server.Application.Dashboard.Queries
{
    public record DashboardSummaryDto(
        decimal TotalBalance,
        decimal MonthlyIncome,
        decimal MonthlyExpenses,
        double IncomeChangePercentage,
        double ExpenseChangePercentage,
        List<MonthlyDataDto> MonthlyData,
        List<CategorySummaryDto> TopSpendingCategories,
        List<TransactionDto> RecentTransactions,
        List<BudgetSummaryDto> TopBudgets
    );

    public record MonthlyDataDto(string Month, decimal Income, decimal Expense);

    public record CategorySummaryDto(string CategoryName, decimal Amount, double Percentage, string? Color);

    public record BudgetSummaryDto(Guid Id, string CategoryName, decimal TotalBudget, decimal SpentAmount, decimal RemainingAmount, double Percentage, string? Icon, string? Color);

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

    public record GetDashboardSummaryQuery(string UserId, DateTimeOffset? ReferenceDate = null) : IRequest<DashboardSummaryDto>;

    public class GetDashboardSummaryQueryHandler : IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryDto>
    {
        private readonly ApplicationDbContext _dbContext;

        public GetDashboardSummaryQueryHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<DashboardSummaryDto> Handle(GetDashboardSummaryQuery request, CancellationToken cancellationToken)
        {
            var referenceDate = request.ReferenceDate ?? DateTimeOffset.UtcNow;
            var currentMonthStart = new DateTime(referenceDate.Year, referenceDate.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var lastMonthStart = currentMonthStart.AddMonths(-1);
            var lastMonthEnd = currentMonthStart.AddDays(-1);

            // 1. Total Balance
            var totalIncomes = (await _dbContext.Incomes
                .Where(i => i.UserId == request.UserId)
                .Select(i => i.Amount)
                .ToListAsync(cancellationToken))
                .Sum();

            var totalExpenses = (await _dbContext.Expenses
                .Where(e => e.UserId == request.UserId)
                .Select(e => e.TotalAmount)
                .ToListAsync(cancellationToken))
                .Sum();

            var totalBalance = totalIncomes - totalExpenses;

            // 2. Monthly Stats
            var monthlyIncome = (await _dbContext.Incomes
                .Where(i => i.UserId == request.UserId && i.Date >= currentMonthStart)
                .Select(i => i.Amount)
                .ToListAsync(cancellationToken))
                .Sum();

            var monthlyExpenses = (await _dbContext.Expenses
                .Where(e => e.UserId == request.UserId && e.Date >= currentMonthStart)
                .Select(e => e.TotalAmount)
                .ToListAsync(cancellationToken))
                .Sum();

            // 3. Last Month Stats for Percentage
            var lastMonthIncome = (await _dbContext.Incomes
                .Where(i => i.UserId == request.UserId && i.Date >= lastMonthStart && i.Date <= lastMonthEnd)
                .Select(i => i.Amount)
                .ToListAsync(cancellationToken))
                .Sum();

            var lastMonthExpenses = (await _dbContext.Expenses
                .Where(e => e.UserId == request.UserId && e.Date >= lastMonthStart && e.Date <= lastMonthEnd)
                .Select(e => e.TotalAmount)
                .ToListAsync(cancellationToken))
                .Sum();

            var incomeChange = lastMonthIncome == 0 ? (monthlyIncome > 0 ? 100 : 0) : (double)((monthlyIncome - lastMonthIncome) / lastMonthIncome) * 100;
            var expenseChange = lastMonthExpenses == 0 ? (monthlyExpenses > 0 ? 100 : 0) : (double)((monthlyExpenses - lastMonthExpenses) / lastMonthExpenses) * 100;

            // 4. Monthly Data (Last 6 Months)
            var monthlyData = new List<MonthlyDataDto>();
            for (int i = 5; i >= 0; i--)
            {
                var monthStart = currentMonthStart.AddMonths(-i);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);
                var monthLabel = monthStart.ToString("MMM", CultureInfo.InvariantCulture);

                var incomeTotal = (await _dbContext.Incomes
                    .Where(inc => inc.UserId == request.UserId && inc.Date >= monthStart && inc.Date <= monthEnd)
                    .Select(inc => inc.Amount)
                    .ToListAsync(cancellationToken))
                    .Sum();

                var expenseTotal = (await _dbContext.Expenses
                    .Where(exp => exp.UserId == request.UserId && exp.Date >= monthStart && exp.Date <= monthEnd)
                    .Select(exp => exp.TotalAmount)
                    .ToListAsync(cancellationToken))
                    .Sum();

                monthlyData.Add(new MonthlyDataDto(monthLabel, incomeTotal, expenseTotal));
            }

            // 5. Top Spending Categories (Current Month)
            var categorySpendingItems = await _dbContext.ExpenseItems
                .Include(ei => ei.Expense)
                .Include(ei => ei.Category)
                .Where(ei => ei.Expense.UserId == request.UserId && ei.Expense.Date >= currentMonthStart)
                .ToListAsync(cancellationToken);

            var categorySpending = categorySpendingItems
                .GroupBy(ei => new { ei.CategoryId, CategoryName = ei.Category.Name, CategoryColor = ei.Category.Color })
                .Select(g => new
                {
                    CategoryName = g.Key.CategoryName,
                    Amount = g.Sum(ei => ei.ItemAmount),
                    Color = g.Key.CategoryColor
                })
                .OrderByDescending(g => g.Amount)
                .Take(3)
                .ToList();

            var topSpendingCategories = categorySpending.Select(cs => new CategorySummaryDto(
                cs.CategoryName,
                cs.Amount,
                monthlyExpenses == 0 ? 0 : (double)(cs.Amount / monthlyExpenses) * 100,
                cs.Color
            )).ToList();

            // 6. Recent Transactions
            var recentIncomes = await _dbContext.Incomes
                .Include(i => i.Category)
                .Where(i => i.UserId == request.UserId)
                .OrderByDescending(i => i.Date)
                .Take(10)
                .Select(i => new TransactionDto(
                    "inc_" + i.Id,
                    i.Source,
                    i.Category.Name,
                    i.Category.Icon,
                    i.Category.Color,
                    i.Date,
                    i.Amount,
                    "Income"
                ))
                .ToListAsync(cancellationToken);

            var recentExpenses = await _dbContext.Expenses
                .Include(e => e.Items)
                    .ThenInclude(ei => ei.Category)
                .Where(e => e.UserId == request.UserId)
                .OrderByDescending(e => e.Date)
                .Take(10)
                .ToListAsync(cancellationToken);

            var mappedExpenses = recentExpenses.Select(e => new TransactionDto(
                "exp_" + e.Id,
                e.Merchant ?? "Expense",
                e.Items.FirstOrDefault()?.Category?.Name ?? "General",
                e.Items.FirstOrDefault()?.Category?.Icon ?? "receipt_long",
                e.Items.FirstOrDefault()?.Category?.Color,
                e.Date,
                -e.TotalAmount,
                "Expense"
            )).ToList();

            var recentTransactions = recentIncomes.Concat(mappedExpenses)
                .OrderByDescending(t => t.Date)
                .Take(10)
                .ToList();

            // 7. Top Budgets (Current Month)
            var currentBudgets = await _dbContext.Budgets
                .Include(b => b.Category)
                .Where(b => b.UserId == request.UserId && b.Month == referenceDate.Month && b.Year == referenceDate.Year)
                .ToListAsync(cancellationToken);

            var categorySpendingMap = categorySpendingItems
                .GroupBy(ei => ei.CategoryId)
                .ToDictionary(g => g.Key, g => g.Sum(ei => ei.ItemAmount));

            var topBudgets = currentBudgets.Select(b =>
            {
                var spent = categorySpendingMap.GetValueOrDefault(b.CategoryId, 0m);
                var remaining = b.Amount - spent;
                var percentage = b.Amount > 0 ? (double)(spent / b.Amount) * 100 : 0;
                
                return new BudgetSummaryDto(
                    b.Id,
                    b.Category?.Name ?? "Unknown",
                    b.Amount,
                    spent,
                    remaining,
                    percentage,
                    b.Category?.Icon,
                    b.Category?.Color
                );
            })
            .OrderByDescending(b => b.Percentage)
            .ToList();

            return new DashboardSummaryDto(
                totalBalance,
                monthlyIncome,
                monthlyExpenses,
                incomeChange,
                expenseChange,
                monthlyData,
                topSpendingCategories,
                recentTransactions,
                topBudgets
            );
        }
    }
}
