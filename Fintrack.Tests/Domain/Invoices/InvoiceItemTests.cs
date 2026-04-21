using FluentAssertions;
using Xunit;
using Fintrack.Server.Domain.Invoices;

namespace Fintrack.Tests.Domain.Invoices;

public class InvoiceItemTests
{
    [Fact]
    public void Create_Should_Succeed_When_Valid()
    {
        var categoryId = Guid.NewGuid();
        var result = InvoiceItem.Create("Product", 2, 3m, 6m, categoryId);

        result.IsSuccess.Should().BeTrue();
        result.Value.ProductName.Should().Be("Product");
        result.Value.Quantity.Should().Be(2);
        result.Value.UnitPrice.Should().Be(3m);
        result.Value.TotalPrice.Should().Be(6m);
        result.Value.AssignedCategoryId.Should().Be(categoryId);
    }

    [Fact]
    public void Create_Should_Fail_When_ProductNameMissing()
    {
        var result = InvoiceItem.Create(" ", 1, 0m, 0m);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(InvoiceItemErrors.ProductNameRequired);
    }

    [Fact]
    public void Create_Should_Fail_When_QuantityNotPositive()
    {
        var result = InvoiceItem.Create("X", 0, 1m, 1m);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(InvoiceItemErrors.InvalidQuantity);
    }
}
