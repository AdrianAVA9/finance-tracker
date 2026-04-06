using Fintrack.Server.Domain.Enums;
using Fintrack.Server.Domain.Expenses;
using Fintrack.Server.Domain.Expenses.Events;
using FluentAssertions;

namespace Fintrack.Tests.Domain.Expenses;

public sealed class ExpenseTests
{
    [Fact]
    public void Create_Should_Succeed_When_ParametersValid()
    {
        var date = DateTime.UtcNow;
        var result = Expense.Create("user-1", 150m, date, "Store A");

        result.IsSuccess.Should().BeTrue();
        result.Value.UserId.Should().Be("user-1");
        result.Value.TotalAmount.Should().Be(150m);
        result.Value.Date.Should().Be(date);
        result.Value.Merchant.Should().Be("Store A");
        result.Value.Status.Should().Be(ExpenseStatus.Completed);
        result.Value.AiExtractionStatus.Should().Be(AiExtractionStatus.NotApplicable);
        result.Value.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_Should_RaiseExpenseCreatedDomainEvent_When_Success()
    {
        var result = Expense.Create("user-1", 50m, DateTime.UtcNow, "Merchant");

        result.IsSuccess.Should().BeTrue();
        var evt = result.Value.GetDomainEvents().Should().ContainSingle().Which;
        evt.Should().BeOfType<ExpenseCreatedDomainEvent>()
            .Which.ExpenseId.Should().Be(result.Value.Id);
    }

    [Fact]
    public void Create_Should_Fail_When_UserIdEmpty()
    {
        var result = Expense.Create("", 100m, DateTime.UtcNow, "Store");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ExpenseErrors.UserIdRequired);
    }

    [Fact]
    public void Create_Should_Fail_When_UserIdWhitespace()
    {
        var result = Expense.Create("   ", 100m, DateTime.UtcNow, "Store");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ExpenseErrors.UserIdRequired);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-0.01)]
    public void Create_Should_Fail_When_TotalAmountNotPositive(decimal amount)
    {
        var result = Expense.Create("user-1", amount, DateTime.UtcNow, "Store");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ExpenseErrors.InvalidTotalAmount);
    }

    [Fact]
    public void Create_Should_UseCustomStatusAndAiStatus()
    {
        var result = Expense.Create(
            "user-1",
            50m,
            DateTime.UtcNow,
            "Receipt Store",
            status: ExpenseStatus.NeedsReview,
            aiExtractionStatus: AiExtractionStatus.Completed);

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(ExpenseStatus.NeedsReview);
        result.Value.AiExtractionStatus.Should().Be(AiExtractionStatus.Completed);
    }

    [Fact]
    public void Update_Should_Succeed_And_RaiseEvent_When_Valid()
    {
        var expense = Expense.Create("user-1", 100m, DateTime.UtcNow, "Old Merchant").Value;
        expense.ClearDomainEvents();

        var newDate = DateTime.UtcNow.AddDays(1);
        var update = expense.Update(200m, newDate, "New Merchant", "INV-001", null);

        update.IsSuccess.Should().BeTrue();
        expense.TotalAmount.Should().Be(200m);
        expense.Date.Should().Be(newDate);
        expense.Merchant.Should().Be("New Merchant");
        expense.InvoiceNumber.Should().Be("INV-001");
        expense.GetDomainEvents().Should().ContainSingle()
            .Which.Should().BeOfType<ExpenseUpdatedDomainEvent>()
            .Which.ExpenseId.Should().Be(expense.Id);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Update_Should_Fail_When_TotalAmountNotPositive(decimal amount)
    {
        var expense = Expense.Create("user-1", 100m, DateTime.UtcNow, "Merchant").Value;
        expense.ClearDomainEvents();

        var update = expense.Update(amount, DateTime.UtcNow, "Merchant", null, null);

        update.IsFailure.Should().BeTrue();
        update.Error.Should().Be(ExpenseErrors.InvalidTotalAmount);
        expense.TotalAmount.Should().Be(100m);
        expense.GetDomainEvents().Should().BeEmpty();
    }

    [Fact]
    public void AddItem_Should_AddToItemsCollection()
    {
        var expense = Expense.Create("user-1", 100m, DateTime.UtcNow, "Store").Value;
        var item = ExpenseItem.Create(Guid.NewGuid(), 100m, "Test item");

        expense.AddItem(item);

        expense.Items.Should().ContainSingle().Which.Should().Be(item);
    }

    [Fact]
    public void ReplaceItems_Should_ClearAndAddNewItems()
    {
        var expense = Expense.Create("user-1", 100m, DateTime.UtcNow, "Store").Value;
        expense.AddItem(ExpenseItem.Create(Guid.NewGuid(), 50m, "Old item 1"));
        expense.AddItem(ExpenseItem.Create(Guid.NewGuid(), 50m, "Old item 2"));

        var newItem = ExpenseItem.Create(Guid.NewGuid(), 100m, "New item");
        expense.ReplaceItems(new[] { newItem });

        expense.Items.Should().ContainSingle().Which.Should().Be(newItem);
    }

    [Fact]
    public void ClearItems_Should_RemoveAllItems()
    {
        var expense = Expense.Create("user-1", 100m, DateTime.UtcNow, "Store").Value;
        expense.AddItem(ExpenseItem.Create(Guid.NewGuid(), 50m, "Item 1"));
        expense.AddItem(ExpenseItem.Create(Guid.NewGuid(), 50m, "Item 2"));

        expense.ClearItems();

        expense.Items.Should().BeEmpty();
    }

    [Fact]
    public void RegisterDeletion_Should_Raise_ExpenseDeletedDomainEvent()
    {
        var expense = Expense.Create("user-1", 100m, DateTime.UtcNow, "Store").Value;
        expense.ClearDomainEvents();

        expense.RegisterDeletion();

        var evt = expense.GetDomainEvents().Should().ContainSingle().Which;
        evt.Should().BeOfType<ExpenseDeletedDomainEvent>()
            .Which.ExpenseId.Should().Be(expense.Id);
    }
}
