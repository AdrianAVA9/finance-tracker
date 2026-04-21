using Fintrack.Server.Domain.Invoices;
using Fintrack.Server.Infrastructure.Repositories;
using FluentAssertions;

namespace Fintrack.Tests.Infrastructure.Repositories;

public sealed class InvoiceRepositoryTests : RepositoryTestBase
{
    [Fact]
    public async Task GetByIdWithItemsAsync_Should_IncludeItems_When_UserMatches()
    {
        using var context = CreateContext();
        var repo = new InvoiceRepository(context);
        var invoice = Invoice.Create("u1", 10m, merchantName: "M").Value;
        invoice.AddItem(InvoiceItem.Create("Line", 1, 10m, 10m).Value);
        repo.Add(invoice);
        await context.SaveChangesAsync();

        var found = await repo.GetByIdWithItemsAsync(invoice.Id, "u1", CancellationToken.None);

        found.Should().NotBeNull();
        found!.Items.Should().ContainSingle();
        found.Items.First().ProductName.Should().Be("Line");
    }

    [Fact]
    public async Task GetByIdAsync_Should_ReturnNull_When_UserDoesNotMatch()
    {
        using var context = CreateContext();
        var repo = new InvoiceRepository(context);
        var invoice = Invoice.Create("owner", 1m).Value;
        repo.Add(invoice);
        await context.SaveChangesAsync();

        var found = await repo.GetByIdAsync(invoice.Id, "other", CancellationToken.None);

        found.Should().BeNull();
    }
}
