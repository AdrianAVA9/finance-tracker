using Fintrack.Server.Domain.Expenses;
using Fintrack.Server.Infrastructure.Repositories;
using Fintrack.Tests.TestData.Budgets;
using Fintrack.Tests.TestData.Expenses;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Fintrack.Tests.Infrastructure.Repositories;

/// <summary>
/// Exercises <see cref="ExpenseRepository"/> against EF Core InMemory (query shapes, filters, includes).
/// Unique constraints and provider-specific SQL belong in integration tests with PostgreSQL.
/// </summary>
public sealed class ExpenseRepositoryTests : RepositoryTestBase
{
    // ═══════════════════════════════════════════════════════════════
    // GetByIdAsync
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public async Task GetByIdAsync_Should_ReturnExpense_When_UserMatches()
    {
        using var context = CreateContext();
        var repo = new ExpenseRepository(context);
        var expense = ExpenseTestDoubles.CreateExpense(userId: "u1");
        context.Expenses.Add(expense);
        await context.SaveChangesAsync();

        var found = await repo.GetByIdAsync(expense.Id, "u1", CancellationToken.None);

        found.Should().NotBeNull();
        found!.Id.Should().Be(expense.Id);
        found.UserId.Should().Be("u1");
    }

    [Fact]
    public async Task GetByIdAsync_Should_ReturnNull_When_UserDoesNotMatch()
    {
        using var context = CreateContext();
        var repo = new ExpenseRepository(context);
        var expense = ExpenseTestDoubles.CreateExpense(userId: "owner");
        context.Expenses.Add(expense);
        await context.SaveChangesAsync();

        var found = await repo.GetByIdAsync(expense.Id, "other", CancellationToken.None);

        found.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_Should_ReturnNull_When_IdDoesNotExist()
    {
        using var context = CreateContext();
        var repo = new ExpenseRepository(context);

        var found = await repo.GetByIdAsync(Guid.NewGuid(), "u1", CancellationToken.None);

        found.Should().BeNull();
    }

    // ═══════════════════════════════════════════════════════════════
    // GetByIdWithItemsAsync
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public async Task GetByIdWithItemsAsync_Should_IncludeItems()
    {
        using var context = CreateContext();
        var repo = new ExpenseRepository(context);
        var expense = ExpenseTestDoubles.CreateExpense(userId: "u1", totalAmount: 100m);
        var categoryId = Guid.NewGuid();
        expense.AddItem(ExpenseItem.Create(categoryId, 60m, "Food"));
        expense.AddItem(ExpenseItem.Create(categoryId, 40m, "Drinks"));
        context.Expenses.Add(expense);
        await context.SaveChangesAsync();

        var found = await repo.GetByIdWithItemsAsync(expense.Id, "u1", CancellationToken.None);

        found.Should().NotBeNull();
        found!.Items.Should().HaveCount(2);
        found.Items.Sum(i => i.ItemAmount).Should().Be(100m);
    }

    [Fact]
    public async Task GetByIdWithItemsAsync_Should_IncludeItemCategory()
    {
        using var context = CreateContext();
        var repo = new ExpenseRepository(context);
        var group = BudgetTestDoubles.CreateGroup(name: "Food");
        var category = BudgetTestDoubles.CreateCategory(name: "Groceries", group: group);
        context.ExpenseCategoryGroups.Add(group);
        context.ExpenseCategories.Add(category);

        var expense = ExpenseTestDoubles.CreateExpense(userId: "u1", totalAmount: 50m);
        expense.AddItem(ExpenseItem.Create(category.Id, 50m, "Weekly shop"));
        context.Expenses.Add(expense);
        await context.SaveChangesAsync();

        var found = await repo.GetByIdWithItemsAsync(expense.Id, "u1", CancellationToken.None);

        found.Should().NotBeNull();
        found!.Items.Should().ContainSingle();
        found.Items.First().Category.Should().NotBeNull();
        found.Items.First().Category!.Name.Should().Be("Groceries");
    }

    [Fact]
    public async Task GetByIdWithItemsAsync_Should_ReturnNull_When_UserDoesNotMatch()
    {
        using var context = CreateContext();
        var repo = new ExpenseRepository(context);
        var expense = ExpenseTestDoubles.CreateExpenseWithItems(userId: "owner");
        context.Expenses.Add(expense);
        await context.SaveChangesAsync();

        var found = await repo.GetByIdWithItemsAsync(expense.Id, "other", CancellationToken.None);

        found.Should().BeNull();
    }

    // ═══════════════════════════════════════════════════════════════
    // SumItemAmountsByCategoryAsync
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public async Task SumItemAmountsByCategoryAsync_Should_SumCorrectly_ForMatchingCategories()
    {
        using var context = CreateContext();
        var repo = new ExpenseRepository(context);
        var catA = Guid.NewGuid();
        var catB = Guid.NewGuid();
        var start = new DateTime(2025, 3, 1);
        var end = new DateTime(2025, 4, 1);

        var expense1 = ExpenseTestDoubles.CreateExpense(userId: "u1", totalAmount: 100m, date: new DateTime(2025, 3, 10));
        expense1.AddItem(ExpenseItem.Create(catA, 60m, null));
        expense1.AddItem(ExpenseItem.Create(catB, 40m, null));

        var expense2 = ExpenseTestDoubles.CreateExpense(userId: "u1", totalAmount: 50m, date: new DateTime(2025, 3, 20));
        expense2.AddItem(ExpenseItem.Create(catA, 50m, null));

        context.Expenses.AddRange(expense1, expense2);
        await context.SaveChangesAsync();

        var sums = await repo.SumItemAmountsByCategoryAsync("u1", start, end, new[] { catA, catB }, CancellationToken.None);

        sums.Should().HaveCount(2);
        sums[catA].Should().Be(110m);
        sums[catB].Should().Be(40m);
    }

    [Fact]
    public async Task SumItemAmountsByCategoryAsync_Should_ExcludeExpensesOutsideDateRange()
    {
        using var context = CreateContext();
        var repo = new ExpenseRepository(context);
        var catA = Guid.NewGuid();
        var start = new DateTime(2025, 3, 1);
        var end = new DateTime(2025, 4, 1);

        var inRange = ExpenseTestDoubles.CreateExpense(userId: "u1", totalAmount: 30m, date: new DateTime(2025, 3, 15));
        inRange.AddItem(ExpenseItem.Create(catA, 30m, null));

        var outOfRange = ExpenseTestDoubles.CreateExpense(userId: "u1", totalAmount: 70m, date: new DateTime(2025, 5, 1));
        outOfRange.AddItem(ExpenseItem.Create(catA, 70m, null));

        context.Expenses.AddRange(inRange, outOfRange);
        await context.SaveChangesAsync();

        var sums = await repo.SumItemAmountsByCategoryAsync("u1", start, end, new[] { catA }, CancellationToken.None);

        sums.Should().ContainSingle();
        sums[catA].Should().Be(30m);
    }

    [Fact]
    public async Task SumItemAmountsByCategoryAsync_Should_ExcludeOtherUsersExpenses()
    {
        using var context = CreateContext();
        var repo = new ExpenseRepository(context);
        var catA = Guid.NewGuid();
        var start = new DateTime(2025, 1, 1);
        var end = new DateTime(2025, 2, 1);

        var mine = ExpenseTestDoubles.CreateExpense(userId: "u1", totalAmount: 20m, date: new DateTime(2025, 1, 10));
        mine.AddItem(ExpenseItem.Create(catA, 20m, null));

        var theirs = ExpenseTestDoubles.CreateExpense(userId: "u2", totalAmount: 80m, date: new DateTime(2025, 1, 10));
        theirs.AddItem(ExpenseItem.Create(catA, 80m, null));

        context.Expenses.AddRange(mine, theirs);
        await context.SaveChangesAsync();

        var sums = await repo.SumItemAmountsByCategoryAsync("u1", start, end, new[] { catA }, CancellationToken.None);

        sums.Should().ContainSingle();
        sums[catA].Should().Be(20m);
    }

    [Fact]
    public async Task SumItemAmountsByCategoryAsync_Should_ReturnEmpty_When_NoCategoryIdsProvided()
    {
        using var context = CreateContext();
        var repo = new ExpenseRepository(context);

        var sums = await repo.SumItemAmountsByCategoryAsync(
            "u1",
            DateTime.MinValue,
            DateTime.MaxValue,
            Array.Empty<Guid>(),
            CancellationToken.None);

        sums.Should().BeEmpty();
    }

    // ═══════════════════════════════════════════════════════════════
    // GetItemsForUserCategoryInDateRangeAsync
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public async Task GetItemsForUserCategoryInDateRangeAsync_Should_FilterByCategoryAndDateRange()
    {
        using var context = CreateContext();
        var repo = new ExpenseRepository(context);
        var targetCat = Guid.NewGuid();
        var otherCat = Guid.NewGuid();
        var start = new DateTime(2025, 6, 1);
        var end = new DateTime(2025, 7, 1);

        var match = ExpenseTestDoubles.CreateExpense(userId: "u1", totalAmount: 50m, date: new DateTime(2025, 6, 15));
        match.AddItem(ExpenseItem.Create(targetCat, 50m, "match"));

        var wrongCat = ExpenseTestDoubles.CreateExpense(userId: "u1", totalAmount: 30m, date: new DateTime(2025, 6, 10));
        wrongCat.AddItem(ExpenseItem.Create(otherCat, 30m, "wrong cat"));

        var wrongDate = ExpenseTestDoubles.CreateExpense(userId: "u1", totalAmount: 25m, date: new DateTime(2025, 8, 1));
        wrongDate.AddItem(ExpenseItem.Create(targetCat, 25m, "wrong date"));

        context.Expenses.AddRange(match, wrongCat, wrongDate);
        await context.SaveChangesAsync();

        var items = await repo.GetItemsForUserCategoryInDateRangeAsync("u1", targetCat, start, end, CancellationToken.None);

        items.Should().ContainSingle();
        items[0].Description.Should().Be("match");
    }

    [Fact]
    public async Task GetItemsForUserCategoryInDateRangeAsync_Should_ExcludeOtherUsers()
    {
        using var context = CreateContext();
        var repo = new ExpenseRepository(context);
        var catId = Guid.NewGuid();
        var start = new DateTime(2025, 1, 1);
        var end = new DateTime(2025, 2, 1);

        var mine = ExpenseTestDoubles.CreateExpense(userId: "u1", totalAmount: 10m, date: new DateTime(2025, 1, 5));
        mine.AddItem(ExpenseItem.Create(catId, 10m, "mine"));

        var theirs = ExpenseTestDoubles.CreateExpense(userId: "u2", totalAmount: 20m, date: new DateTime(2025, 1, 5));
        theirs.AddItem(ExpenseItem.Create(catId, 20m, "theirs"));

        context.Expenses.AddRange(mine, theirs);
        await context.SaveChangesAsync();

        var items = await repo.GetItemsForUserCategoryInDateRangeAsync("u1", catId, start, end, CancellationToken.None);

        items.Should().ContainSingle();
        items[0].Description.Should().Be("mine");
    }

    [Fact]
    public async Task GetItemsForUserCategoryInDateRangeAsync_Should_OrderByDateDescending()
    {
        using var context = CreateContext();
        var repo = new ExpenseRepository(context);
        var catId = Guid.NewGuid();
        var start = new DateTime(2025, 3, 1);
        var end = new DateTime(2025, 4, 1);

        var early = ExpenseTestDoubles.CreateExpense(userId: "u1", totalAmount: 10m, date: new DateTime(2025, 3, 5));
        early.AddItem(ExpenseItem.Create(catId, 10m, "early"));

        var late = ExpenseTestDoubles.CreateExpense(userId: "u1", totalAmount: 20m, date: new DateTime(2025, 3, 25));
        late.AddItem(ExpenseItem.Create(catId, 20m, "late"));

        var mid = ExpenseTestDoubles.CreateExpense(userId: "u1", totalAmount: 15m, date: new DateTime(2025, 3, 15));
        mid.AddItem(ExpenseItem.Create(catId, 15m, "mid"));

        context.Expenses.AddRange(early, late, mid);
        await context.SaveChangesAsync();

        var items = await repo.GetItemsForUserCategoryInDateRangeAsync("u1", catId, start, end, CancellationToken.None);

        items.Should().HaveCount(3);
        items.Select(i => i.Description).Should().ContainInOrder("late", "mid", "early");
    }

    // ═══════════════════════════════════════════════════════════════
    // Add
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public void Add_Should_TrackNewExpense()
    {
        using var context = CreateContext();
        var repo = new ExpenseRepository(context);
        var expense = ExpenseTestDoubles.CreateExpense();

        repo.Add(expense);

        context.ChangeTracker.Entries<Expense>()
            .Should().ContainSingle()
            .Which.State.Should().Be(EntityState.Added);
    }

    [Fact]
    public async Task Add_Should_PersistExpenseWithItems()
    {
        using var context = CreateContext();
        var repo = new ExpenseRepository(context);
        var expense = ExpenseTestDoubles.CreateExpense(totalAmount: 75m);
        expense.AddItem(ExpenseItem.Create(Guid.NewGuid(), 75m, "Lunch"));

        repo.Add(expense);
        await context.SaveChangesAsync();

        var persisted = await context.Expenses
            .Include(e => e.Items)
            .SingleAsync(e => e.Id == expense.Id);

        persisted.TotalAmount.Should().Be(75m);
        persisted.Items.Should().ContainSingle()
            .Which.Description.Should().Be("Lunch");
    }

    // ═══════════════════════════════════════════════════════════════
    // Update
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public async Task Update_Should_PersistModifiedFields()
    {
        using var context = CreateContext();
        var repo = new ExpenseRepository(context);
        var expense = ExpenseTestDoubles.CreateExpense(userId: "u1", totalAmount: 50m, merchant: "Old");
        context.Expenses.Add(expense);
        await context.SaveChangesAsync();

        expense.Update(75m, expense.Date, "New", null, null);
        repo.Update(expense);
        await context.SaveChangesAsync();

        var found = await context.Expenses.SingleAsync(e => e.Id == expense.Id);
        found.TotalAmount.Should().Be(75m);
        found.Merchant.Should().Be("New");
    }

    // ═══════════════════════════════════════════════════════════════
    // Remove
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public async Task Remove_Should_DeleteExpense_FromContext()
    {
        using var context = CreateContext();
        var repo = new ExpenseRepository(context);
        var expense = ExpenseTestDoubles.CreateExpense();
        context.Expenses.Add(expense);
        await context.SaveChangesAsync();

        repo.Remove(expense);
        await context.SaveChangesAsync();

        context.Expenses.Local.Should().BeEmpty();
        (await context.Expenses.AnyAsync(e => e.Id == expense.Id)).Should().BeFalse();
    }

    [Fact]
    public async Task Remove_Should_CascadeDeleteItems()
    {
        using var context = CreateContext();
        var repo = new ExpenseRepository(context);
        var expense = ExpenseTestDoubles.CreateExpenseWithItems(totalAmount: 100m, itemCount: 3);
        context.Expenses.Add(expense);
        await context.SaveChangesAsync();

        repo.Remove(expense);
        await context.SaveChangesAsync();

        (await context.ExpenseItems.AnyAsync()).Should().BeFalse();
    }
}
