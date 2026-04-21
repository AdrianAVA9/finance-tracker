using Fintrack.Server.Domain.Budgets;
using Fintrack.Server.Domain.ExpenseCategories;
using Fintrack.Server.Infrastructure.Repositories;
using Fintrack.Tests.TestData.Budgets;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Fintrack.Tests.Infrastructure.Repositories;

/// <summary>
/// Exercises <see cref="BudgetRepository"/> against EF Core InMemory (query shapes, filters, includes).
/// Unique constraints and provider-specific SQL belong in integration tests with the real database.
/// </summary>
public sealed class BudgetRepositoryTests : RepositoryTestBase
{

    [Fact]
    public async Task GetByIdAsync_Should_ReturnBudget_When_UserMatches()
    {
        using var context = CreateContext();
        var repo = new BudgetRepository(context);
        var budget = BudgetTestDoubles.CreateBudget("u1", Guid.NewGuid(), 100m, month: 5, year: 2024);
        context.Budgets.Add(budget);
        await context.SaveChangesAsync();

        var found = await repo.GetByIdAsync(budget.Id, "u1", CancellationToken.None);

        found.Should().NotBeNull();
        found!.Id.Should().Be(budget.Id);
    }

    [Fact]
    public async Task GetByIdAsync_Should_ReturnNull_When_UserDoesNotMatch()
    {
        using var context = CreateContext();
        var repo = new BudgetRepository(context);
        var budget = BudgetTestDoubles.CreateBudget("owner", Guid.NewGuid(), 100m, month: 5, year: 2024);
        context.Budgets.Add(budget);
        await context.SaveChangesAsync();

        var found = await repo.GetByIdAsync(budget.Id, "other", CancellationToken.None);

        found.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdWithCategoryAsync_Should_IncludeCategoryAndGroup()
    {
        using var context = CreateContext();
        var repo = new BudgetRepository(context);
        var group = BudgetTestDoubles.CreateGroup(name: "G1");
        var category = BudgetTestDoubles.CreateCategory(name: "C1", group: group);
        context.ExpenseCategoryGroups.Add(group);
        context.ExpenseCategories.Add(category);
        var budget = BudgetTestDoubles.CreateBudget("u1", category.Id, 50m, month: 1, year: 2024);
        context.Budgets.Add(budget);
        await context.SaveChangesAsync();

        var found = await repo.GetByIdWithCategoryAsync(budget.Id, "u1", CancellationToken.None);

        found.Should().NotBeNull();
        found!.Category.Should().NotBeNull();
        found.Category!.Name.Should().Be("C1");
        found.Category.Group.Should().NotBeNull();
        found.Category.Group!.Name.Should().Be("G1");
    }

    [Fact]
    public async Task GetUserBudgetsByMonthAsync_Should_FilterByUserMonthAndYear()
    {
        using var context = CreateContext();
        var repo = new BudgetRepository(context);
        var categoryA = Guid.NewGuid();
        var categoryB = Guid.NewGuid();
        var a = BudgetTestDoubles.CreateBudget("u1", categoryA, 10m, month: 3, year: 2024);
        var b = BudgetTestDoubles.CreateBudget("u1", categoryB, 20m, month: 4, year: 2024);
        var c = BudgetTestDoubles.CreateBudget("u2", categoryA, 30m, month: 3, year: 2024);
        context.Budgets.AddRange(a, b, c);
        await context.SaveChangesAsync();

        var list = await repo.GetUserBudgetsByMonthAsync("u1", 3, 2024, CancellationToken.None);

        list.Should().ContainSingle();
        list[0].CategoryId.Should().Be(categoryA);
    }

    [Fact]
    public async Task ExistsAsync_Should_ReturnTrue_When_SlotTaken()
    {
        using var context = CreateContext();
        var repo = new BudgetRepository(context);
        var categoryId = Guid.NewGuid();
        var budget = BudgetTestDoubles.CreateBudget("u1", categoryId, 1m, month: 6, year: 2025);
        context.Budgets.Add(budget);
        await context.SaveChangesAsync();

        var exists = await repo.ExistsAsync("u1", categoryId, 6, 2025, CancellationToken.None);

        exists.Should().BeTrue();
    }

    [Fact]
    public async Task GetRecurrentBudgetsForMonthForAllUsersAsync_Should_ReturnOnlyRecurrentForMonth()
    {
        using var context = CreateContext();
        var repo = new BudgetRepository(context);
        var recurrent = BudgetTestDoubles.CreateBudget("a", Guid.NewGuid(), 100m, isRecurrent: true, month: 2, year: 2024);
        var nonRec = BudgetTestDoubles.CreateBudget("b", Guid.NewGuid(), 200m, isRecurrent: false, month: 2, year: 2024);
        var otherMonth = BudgetTestDoubles.CreateBudget("c", Guid.NewGuid(), 300m, isRecurrent: true, month: 3, year: 2024);
        context.Budgets.AddRange(recurrent, nonRec, otherMonth);
        await context.SaveChangesAsync();

        var list = await repo.GetRecurrentBudgetsForMonthForAllUsersAsync(2, 2024, CancellationToken.None);

        list.Should().ContainSingle();
        list[0].UserId.Should().Be("a");
        list[0].IsRecurrent.Should().BeTrue();
    }

    [Fact]
    public void Remove_Should_DetachEntity_FromContext()
    {
        using var context = CreateContext();
        var repo = new BudgetRepository(context);
        var budget = BudgetTestDoubles.CreateBudget("u1", Guid.NewGuid(), 1m, month: 1, year: 2024);
        context.Budgets.Add(budget);
        context.SaveChanges();

        repo.Remove(budget);
        context.SaveChanges();

        context.Budgets.Local.Should().BeEmpty();
        context.Budgets.Any(b => b.Id == budget.Id).Should().BeFalse();
    }
}
