using Fintrack.Server.Application.Dashboard.Queries;
using Fintrack.Server.Domain.Enums;
using Fintrack.Server.Domain.IncomeCategories;
using Fintrack.Server.Domain.Incomes;
using Fintrack.Server.Domain.Expenses;
using Fintrack.Tests.Infrastructure.Repositories;
using FluentAssertions;

namespace Fintrack.Tests.Application.Dashboard.Queries;

public sealed class GetDashboardSummaryQueryHandlerTests : RepositoryTestBase
{
    [Fact]
    public async Task Handle_Should_SetTotalBalance_AsCurrentMonthNet_ExcludingPriorMonths()
    {
        using var context = CreateContext();
        var userId = "user-1";
        var incomeCat = IncomeCategory.Create("Cat", "i", "#fff", userId: null, isEditable: false).Value;
        context.IncomeCategories.Add(incomeCat);
        await context.SaveChangesAsync();

        var june2024 = new DateTime(2024, 6, 15, 0, 0, 0, DateTimeKind.Utc);
        var may2024 = new DateTime(2024, 5, 10, 0, 0, 0, DateTimeKind.Utc);

        var incomeJune = Income.Create(userId, "job", 500m, incomeCat.Id, june2024, null).Value;
        var incomeMay = Income.Create(userId, "old", 10_000m, incomeCat.Id, may2024, null).Value;
        var expenseJune = Expense.Create(userId, 200m, june2024, "store", status: ExpenseStatus.Completed).Value;
        var expenseMay = Expense.Create(userId, 50m, may2024, "old", status: ExpenseStatus.Completed).Value;

        context.Incomes.AddRange(incomeJune, incomeMay);
        context.Expenses.AddRange(expenseJune, expenseMay);
        await context.SaveChangesAsync();

        var handler = new GetDashboardSummaryQueryHandler(context);
        var refDate = new DateTimeOffset(2024, 6, 20, 10, 0, 0, TimeSpan.Zero);
        var result = await handler.Handle(new GetDashboardSummaryQuery(userId, refDate), CancellationToken.None);

        result.MonthlyIncome.Should().Be(500m);
        result.MonthlyExpenses.Should().Be(200m);
        result.TotalBalance.Should().Be(300m);
    }
}
