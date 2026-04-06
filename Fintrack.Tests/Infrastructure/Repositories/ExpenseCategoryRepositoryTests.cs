using Fintrack.Server.Domain.ExpenseCategories;
using Fintrack.Server.Infrastructure.Repositories;
using Fintrack.Tests.TestData.Budgets;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Fintrack.Tests.Infrastructure.Repositories;

/// <summary>
/// Exercises <see cref="ExpenseCategoryRepository"/> against EF Core InMemory (query shapes, filters, includes).
/// Provider-specific SQL and unique constraints belong in integration tests with PostgreSQL.
/// </summary>
public sealed class ExpenseCategoryRepositoryTests : RepositoryTestBase
{

    private static ExpenseCategory CreateCategory(
        string name = "Test Category",
        string? userId = null,
        ExpenseCategoryGroup? group = null,
        bool isEditable = true)
    {
        return BudgetTestDoubles.CreateCategory(
            name: name,
            group: group ?? BudgetTestDoubles.CreateGroup());
    }

    [Fact]
    public async Task GetByIdAsync_Should_ReturnCategory_When_Exists()
    {
        using var context = CreateContext();
        var repo = new ExpenseCategoryRepository(context);
        var group = BudgetTestDoubles.CreateGroup(name: "G1");
        var category = BudgetTestDoubles.CreateCategory(name: "Food", group: group);
        context.ExpenseCategoryGroups.Add(group);
        context.ExpenseCategories.Add(category);
        await context.SaveChangesAsync();

        var found = await repo.GetByIdAsync(category.Id, CancellationToken.None);

        found.Should().NotBeNull();
        found!.Id.Should().Be(category.Id);
        found.Name.Should().Be("Food");
    }

    [Fact]
    public async Task GetByIdAsync_Should_ReturnNull_When_Missing()
    {
        using var context = CreateContext();
        var repo = new ExpenseCategoryRepository(context);

        var found = await repo.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        found.Should().BeNull();
    }

    [Fact]
    public async Task GetAllByUserIdAsync_Should_Return_SystemCategories_And_UserCategories()
    {
        using var context = CreateContext();
        var repo = new ExpenseCategoryRepository(context);
        var group = BudgetTestDoubles.CreateGroup(name: "General");
        context.ExpenseCategoryGroups.Add(group);

        var system = ExpenseCategory.Create("Alpha", null, null, null, group.Id, userId: null, isEditable: false).Value;
        var userOwned = ExpenseCategory.Create("Beta", null, null, null, group.Id, userId: "user-1", isEditable: true).Value;

        context.ExpenseCategories.AddRange(system, userOwned);
        await context.SaveChangesAsync();

        var list = await repo.GetAllByUserIdAsync("user-1", CancellationToken.None);

        list.Should().HaveCount(2);
        list.Select(c => c.Name).Should().ContainInOrder("Alpha", "Beta");
    }

    [Fact]
    public async Task GetAllByUserIdAsync_Should_Not_Return_OtherUsersCategories()
    {
        using var context = CreateContext();
        var repo = new ExpenseCategoryRepository(context);
        var group = BudgetTestDoubles.CreateGroup(name: "General");
        context.ExpenseCategoryGroups.Add(group);

        var system = ExpenseCategory.Create("Shared", null, null, null, group.Id, userId: null, isEditable: false).Value;
        var otherUser = ExpenseCategory.Create("Private", null, null, null, group.Id, userId: "other", isEditable: true).Value;

        context.ExpenseCategories.AddRange(system, otherUser);
        await context.SaveChangesAsync();

        var list = await repo.GetAllByUserIdAsync("user-1", CancellationToken.None);

        list.Should().ContainSingle();
        list[0].Name.Should().Be("Shared");
        list.Should().NotContain(c => c.UserId == "other");
    }

    [Fact]
    public async Task GetAllByUserIdAsync_Should_OrderByName()
    {
        using var context = CreateContext();
        var repo = new ExpenseCategoryRepository(context);
        var group = BudgetTestDoubles.CreateGroup(name: "General");
        context.ExpenseCategoryGroups.Add(group);

        var zulu = ExpenseCategory.Create("Zulu", null, null, null, group.Id, userId: "u1", isEditable: true).Value;
        var alpha = ExpenseCategory.Create("Alpha", null, null, null, group.Id, userId: "u1", isEditable: true).Value;
        var mid = ExpenseCategory.Create("Mid", null, null, null, group.Id, userId: "u1", isEditable: true).Value;

        context.ExpenseCategories.AddRange(zulu, alpha, mid);
        await context.SaveChangesAsync();

        var list = await repo.GetAllByUserIdAsync("u1", CancellationToken.None);

        list.Select(c => c.Name).Should().ContainInOrder("Alpha", "Mid", "Zulu");
    }

    [Fact]
    public async Task GetAllByUserIdAsync_Should_IncludeGroup()
    {
        using var context = CreateContext();
        var repo = new ExpenseCategoryRepository(context);
        var group = BudgetTestDoubles.CreateGroup(name: "Housing");
        context.ExpenseCategoryGroups.Add(group);

        var category = ExpenseCategory.Create("Rent", null, null, null, group.Id, userId: "u1", isEditable: true).Value;
        context.ExpenseCategories.Add(category);
        await context.SaveChangesAsync();

        var list = await repo.GetAllByUserIdAsync("u1", CancellationToken.None);

        list.Should().ContainSingle();
        list[0].Group.Should().NotBeNull();
        list[0].Group!.Name.Should().Be("Housing");
    }

    [Fact]
    public async Task GetAllByUserIdAsync_Should_AsNoTracking_NotTrackEntities()
    {
        using var context = CreateContext();
        var repo = new ExpenseCategoryRepository(context);
        var group = BudgetTestDoubles.CreateGroup(name: "G");
        context.ExpenseCategoryGroups.Add(group);

        var category = ExpenseCategory.Create("Tracked", null, null, null, group.Id, userId: "u1", isEditable: true).Value;
        context.ExpenseCategories.Add(category);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        await repo.GetAllByUserIdAsync("u1", CancellationToken.None);

        context.ChangeTracker.Entries<ExpenseCategory>().Should().BeEmpty();
    }

    [Fact]
    public async Task ExistsAsync_Should_ReturnTrue_When_Present()
    {
        using var context = CreateContext();
        var repo = new ExpenseCategoryRepository(context);
        var group = BudgetTestDoubles.CreateGroup();
        var category = BudgetTestDoubles.CreateCategory(group: group);
        context.ExpenseCategoryGroups.Add(group);
        context.ExpenseCategories.Add(category);
        await context.SaveChangesAsync();

        var exists = await repo.ExistsAsync(category.Id, CancellationToken.None);

        exists.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_Should_ReturnFalse_When_Absent()
    {
        using var context = CreateContext();
        var repo = new ExpenseCategoryRepository(context);

        var exists = await repo.ExistsAsync(Guid.NewGuid(), CancellationToken.None);

        exists.Should().BeFalse();
    }

    [Fact]
    public void Add_Should_TrackNewEntity()
    {
        using var context = CreateContext();
        var repo = new ExpenseCategoryRepository(context);
        var group = BudgetTestDoubles.CreateGroup();
        var category = BudgetTestDoubles.CreateCategory(group: group);

        repo.Add(category);

        context.ChangeTracker.Entries<ExpenseCategory>()
            .Should().ContainSingle()
            .Which.State.Should().Be(EntityState.Added);
    }

    [Fact]
    public void Remove_Should_DetachEntity_FromContext()
    {
        using var context = CreateContext();
        var repo = new ExpenseCategoryRepository(context);
        var group = BudgetTestDoubles.CreateGroup();
        var category = BudgetTestDoubles.CreateCategory(group: group);
        context.ExpenseCategoryGroups.Add(group);
        context.ExpenseCategories.Add(category);
        context.SaveChanges();

        repo.Remove(category);
        context.SaveChanges();

        context.ExpenseCategories.Local.Should().BeEmpty();
        context.ExpenseCategories.Any(c => c.Id == category.Id).Should().BeFalse();
    }
}
