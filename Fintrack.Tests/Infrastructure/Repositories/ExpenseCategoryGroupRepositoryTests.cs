using Fintrack.Server.Domain.ExpenseCategories;
using Fintrack.Server.Infrastructure.Data;
using Fintrack.Server.Infrastructure.Repositories;
using Fintrack.Tests.TestData.Budgets;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace Fintrack.Tests.Infrastructure.Repositories;

/// <summary>
/// Exercises <see cref="ExpenseCategoryGroupRepository"/> against EF Core InMemory (query shapes, filters).
/// Provider-specific SQL belongs in integration tests with PostgreSQL.
/// </summary>
public sealed class ExpenseCategoryGroupRepositoryTests
{
    private static ApplicationDbContext CreateContext()
    {
        var publisher = Substitute.For<IPublisher>();
        publisher.Publish(Arg.Any<INotification>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options, publisher);
    }

    [Fact]
    public async Task GetByIdAsync_Should_ReturnGroup_When_Exists()
    {
        using var context = CreateContext();
        var repo = new ExpenseCategoryGroupRepository(context);
        var group = BudgetTestDoubles.CreateGroup(name: "My group");
        context.ExpenseCategoryGroups.Add(group);
        await context.SaveChangesAsync();

        var found = await repo.GetByIdAsync(group.Id, CancellationToken.None);

        found.Should().NotBeNull();
        found!.Id.Should().Be(group.Id);
        found.Name.Should().Be("My group");
    }

    [Fact]
    public async Task GetByIdAsync_Should_ReturnNull_When_Missing()
    {
        using var context = CreateContext();
        var repo = new ExpenseCategoryGroupRepository(context);

        var found = await repo.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        found.Should().BeNull();
    }

    [Fact]
    public async Task GetAllByUserIdAsync_Should_Return_SystemGroups_And_UserGroups_OrderedByName()
    {
        using var context = CreateContext();
        var repo = new ExpenseCategoryGroupRepository(context);

        var systemA = ExpenseCategoryGroup.CreateForSystem("Alpha", null).Value;
        var systemB = ExpenseCategoryGroup.CreateForSystem("Beta", null).Value;
        var userOwned = ExpenseCategoryGroup.CreateForUser("Zeta", null, "user-1").Value;

        context.ExpenseCategoryGroups.AddRange(systemA, systemB, userOwned);
        await context.SaveChangesAsync();

        var list = await repo.GetAllByUserIdAsync("user-1", CancellationToken.None);

        list.Should().HaveCount(3);
        list.Select(g => g.Name).Should().ContainInOrder("Alpha", "Beta", "Zeta");
    }

    [Fact]
    public async Task GetAllByUserIdAsync_Should_Not_Return_OtherUsersPrivateGroups()
    {
        using var context = CreateContext();
        var repo = new ExpenseCategoryGroupRepository(context);

        var system = ExpenseCategoryGroup.CreateForSystem("Shared", null).Value;
        var otherUser = ExpenseCategoryGroup.CreateForUser("Private", null, "other").Value;

        context.ExpenseCategoryGroups.AddRange(system, otherUser);
        await context.SaveChangesAsync();

        var list = await repo.GetAllByUserIdAsync("user-1", CancellationToken.None);

        list.Should().ContainSingle();
        list[0].Name.Should().Be("Shared");
        list.Should().NotContain(g => g.UserId == "other");
    }

    [Fact]
    public async Task GetAllByUserIdAsync_Should_AsNoTracking_NotTrackEntities()
    {
        using var context = CreateContext();
        var repo = new ExpenseCategoryGroupRepository(context);
        var group = BudgetTestDoubles.CreateGroup(name: "Tracked seed");
        context.ExpenseCategoryGroups.Add(group);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        await repo.GetAllByUserIdAsync("any", CancellationToken.None);

        context.ChangeTracker.Entries<ExpenseCategoryGroup>().Should().BeEmpty();
    }

    [Fact]
    public async Task ExistsAsync_Should_ReturnTrue_When_Present()
    {
        using var context = CreateContext();
        var repo = new ExpenseCategoryGroupRepository(context);
        var group = BudgetTestDoubles.CreateGroup();
        context.ExpenseCategoryGroups.Add(group);
        await context.SaveChangesAsync();

        var exists = await repo.ExistsAsync(group.Id, CancellationToken.None);

        exists.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_Should_ReturnFalse_When_Absent()
    {
        using var context = CreateContext();
        var repo = new ExpenseCategoryGroupRepository(context);

        var exists = await repo.ExistsAsync(Guid.NewGuid(), CancellationToken.None);

        exists.Should().BeFalse();
    }

    [Fact]
    public void Remove_Should_DetachEntity_FromContext()
    {
        using var context = CreateContext();
        var repo = new ExpenseCategoryGroupRepository(context);
        var group = BudgetTestDoubles.CreateGroup();
        context.ExpenseCategoryGroups.Add(group);
        context.SaveChanges();

        repo.Remove(group);
        context.SaveChanges();

        context.ExpenseCategoryGroups.Local.Should().BeEmpty();
        context.ExpenseCategoryGroups.Any(g => g.Id == group.Id).Should().BeFalse();
    }
}
