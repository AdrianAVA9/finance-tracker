using Fintrack.Server.Domain.Expenses;
using FluentAssertions;

namespace Fintrack.Tests.Domain.Expenses;

public sealed class ExpenseItemTests
{
    [Fact]
    public void Create_Should_ReturnExpenseItemWithCorrectValues()
    {
        var categoryId = Guid.NewGuid();

        var item = ExpenseItem.Create(categoryId, 25.50m, "Coffee");

        item.CategoryId.Should().Be(categoryId);
        item.ItemAmount.Should().Be(25.50m);
        item.Description.Should().Be("Coffee");
        item.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Update_Should_ModifyProperties()
    {
        var item = ExpenseItem.Create(Guid.NewGuid(), 10m, "Original");
        var newCategoryId = Guid.NewGuid();

        item.Update(newCategoryId, 20m, "Updated");

        item.CategoryId.Should().Be(newCategoryId);
        item.ItemAmount.Should().Be(20m);
        item.Description.Should().Be("Updated");
    }

    [Fact]
    public void Create_Should_GenerateUniqueIds()
    {
        var item1 = ExpenseItem.Create(Guid.NewGuid(), 10m, "Item 1");
        var item2 = ExpenseItem.Create(Guid.NewGuid(), 20m, "Item 2");

        item1.Id.Should().NotBe(item2.Id);
    }
}
