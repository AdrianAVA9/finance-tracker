using FluentAssertions;
using Xunit;
using Fintrack.Server.Domain.Invoices;

namespace Fintrack.Tests.Domain.Invoices;

public class InvoiceTests
{
    [Fact]
    public void Create_Should_Succeed_When_Valid()
    {
        var result = Invoice.Create("user-1", 10.5m, merchantName: "Store", date: DateTime.UtcNow);

        result.IsSuccess.Should().BeTrue();
        result.Value.UserId.Should().Be("user-1");
        result.Value.TotalAmount.Should().Be(10.5m);
        result.Value.MerchantName.Should().Be("Store");
        result.Value.Status.Should().Be("Pending");
        result.Value.Items.Should().BeEmpty();
    }

    [Fact]
    public void Create_Should_Fail_When_UserIdMissing()
    {
        var result = Invoice.Create("  ", 1m);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(InvoiceErrors.UserIdRequired);
    }

    [Fact]
    public void Create_Should_Fail_When_TotalNegative()
    {
        var result = Invoice.Create("user-1", -0.01m);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(InvoiceErrors.NegativeTotalAmount);
    }

    [Fact]
    public void Create_Should_Fail_When_StatusEmpty()
    {
        var result = Invoice.Create("user-1", 1m, status: "  ");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(InvoiceErrors.StatusRequired);
    }

    [Fact]
    public void AddItem_Should_AppendLine()
    {
        var invoice = Invoice.Create("user-1", 5m).Value;
        var item = InvoiceItem.Create("A", 1, 2m, 2m).Value;

        invoice.AddItem(item);

        invoice.Items.Should().ContainSingle().Which.Should().BeSameAs(item);
    }
}
