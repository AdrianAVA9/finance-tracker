using Fintrack.Server.Domain.IncomeCategories;
using Fintrack.Server.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Fintrack.Tests.Infrastructure.Repositories;

public sealed class IncomeCategoryRepositoryTests : RepositoryTestBase
{
    [Fact]
    public async Task GetByIdAsync_Should_ReturnCategory_When_Exists()
    {
        using var context = CreateContext();
        var repo = new IncomeCategoryRepository(context);
        var category = IncomeCategory.Create("Pay", "p", "#fff", null, false).Value;
        context.IncomeCategories.Add(category);
        await context.SaveChangesAsync();

        var found = await repo.GetByIdAsync(category.Id, CancellationToken.None);

        found.Should().NotBeNull();
        found!.Id.Should().Be(category.Id);
        found.Name.Should().Be("Pay");
    }

    [Fact]
    public async Task GetAllByUserIdAsync_Should_Return_SystemCategories_And_UserCategories_OrderedByName()
    {
        using var context = CreateContext();
        var repo = new IncomeCategoryRepository(context);

        var system = IncomeCategory.Create("Zeta", null, null, null, false).Value;
        var userOwned = IncomeCategory.Create("Alpha", null, null, "user-1", true).Value;

        context.IncomeCategories.AddRange(system, userOwned);
        await context.SaveChangesAsync();

        var list = await repo.GetAllByUserIdAsync("user-1", CancellationToken.None);

        list.Should().HaveCount(2);
        list.Select(c => c.Name).Should().ContainInOrder("Alpha", "Zeta");
    }

    [Fact]
    public async Task GetAllByUserIdAsync_Should_AsNoTracking_NotTrackEntities()
    {
        using var context = CreateContext();
        var repo = new IncomeCategoryRepository(context);
        var category = IncomeCategory.Create("X", null, null, null, false).Value;
        context.IncomeCategories.Add(category);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        await repo.GetAllByUserIdAsync("any", CancellationToken.None);

        context.ChangeTracker.Entries<IncomeCategory>().Should().BeEmpty();
    }

    [Fact]
    public async Task ExistsAsync_Should_ReturnTrue_When_Present()
    {
        using var context = CreateContext();
        var repo = new IncomeCategoryRepository(context);
        var category = IncomeCategory.Create("E", null, null, null, false).Value;
        context.IncomeCategories.Add(category);
        await context.SaveChangesAsync();

        var exists = await repo.ExistsAsync(category.Id, CancellationToken.None);

        exists.Should().BeTrue();
    }
}
