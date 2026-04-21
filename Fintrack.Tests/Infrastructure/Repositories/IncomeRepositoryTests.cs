using Fintrack.Server.Domain.IncomeCategories;
using Fintrack.Server.Domain.Incomes;
using Fintrack.Server.Infrastructure.Data;
using Fintrack.Server.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Fintrack.Tests.Infrastructure.Repositories;

public sealed class IncomeRepositoryTests : RepositoryTestBase
{
    private static async Task<IncomeCategory> SeedCategoryAsync(ApplicationDbContext context)
    {
        var cat = IncomeCategory.Create("Cat", "i", "#fff", userId: null, isEditable: false).Value;
        context.IncomeCategories.Add(cat);
        await context.SaveChangesAsync();
        return cat;
    }

    [Fact]
    public async Task GetByIdAsync_Should_ReturnIncome_When_UserMatches()
    {
        using var context = CreateContext();
        var cat = await SeedCategoryAsync(context);
        var repo = new IncomeRepository(context);
        var income = Income.Create("u1", "src", 10m, cat.Id, DateTime.UtcNow.Date, null).Value;
        context.Incomes.Add(income);
        await context.SaveChangesAsync();

        var found = await repo.GetByIdAsync(income.Id, "u1", CancellationToken.None);

        found.Should().NotBeNull();
        found!.Id.Should().Be(income.Id);
    }

    [Fact]
    public async Task GetByIdAsync_Should_ReturnNull_When_UserDoesNotMatch()
    {
        using var context = CreateContext();
        var cat = await SeedCategoryAsync(context);
        var repo = new IncomeRepository(context);
        var income = Income.Create("owner", "src", 10m, cat.Id, DateTime.UtcNow.Date, null).Value;
        context.Incomes.Add(income);
        await context.SaveChangesAsync();

        var found = await repo.GetByIdAsync(income.Id, "other", CancellationToken.None);

        found.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsNoTrackingAsync_Should_ReturnSameRow_As_GetByIdAsync()
    {
        using var context = CreateContext();
        var cat = await SeedCategoryAsync(context);
        var repo = new IncomeRepository(context);
        var income = Income.Create("u1", "src", 10m, cat.Id, DateTime.UtcNow.Date, null).Value;
        context.Incomes.Add(income);
        await context.SaveChangesAsync();

        var noTrack = await repo.GetByIdAsNoTrackingAsync(income.Id, "u1", CancellationToken.None);
        var tracked = await repo.GetByIdAsync(income.Id, "u1", CancellationToken.None);

        noTrack.Should().NotBeNull();
        tracked.Should().NotBeNull();
        noTrack!.Id.Should().Be(tracked!.Id);
        noTrack.Amount.Should().Be(tracked.Amount);
    }
}
