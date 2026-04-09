using Fintrack.Server.Application.Expenses.Commands.CreateExpense;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace Fintrack.Tests.Application.Expenses.Commands.CreateExpense;

public sealed class CreateExpenseCommandValidatorTests
{
    private readonly CreateExpenseCommandValidator _validator = new();

    private static CreateExpenseCommand CreateValidCommand() =>
        new(
            UserId: "user-1",
            TotalAmount: 100m,
            Date: DateTime.UtcNow,
            Merchant: "Store",
            InvoiceNumber: null,
            InvoiceImageUrl: null,
            Items: new List<ExpenseItemDto>
            {
                new(Guid.NewGuid(), 100m, "Item")
            },
            Invoice: null);

    [Fact]
    public void Should_Pass_When_CommandIsValid()
    {
        var result = _validator.TestValidate(CreateValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Fail_When_UserIdEmpty()
    {
        var command = CreateValidCommand() with { UserId = "" };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Should_Fail_When_TotalAmountZero()
    {
        var command = CreateValidCommand() with { TotalAmount = 0 };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.TotalAmount);
    }

    [Fact]
    public void Should_Fail_When_ItemsEmpty()
    {
        var command = CreateValidCommand() with { Items = new List<ExpenseItemDto>() };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Items);
    }

    [Fact]
    public void Should_Fail_When_ItemsSumDoesNotMatchTotal()
    {
        var command = CreateValidCommand() with
        {
            TotalAmount = 100m,
            Items = new List<ExpenseItemDto>
            {
                new(Guid.NewGuid(), 50m, "Item 1"),
                new(Guid.NewGuid(), 30m, "Item 2")
            }
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor("MathematicalIntegrity");
    }

    [Fact]
    public void Should_Fail_When_ItemCategoryIdEmpty()
    {
        var command = CreateValidCommand() with
        {
            Items = new List<ExpenseItemDto>
            {
                new(Guid.Empty, 100m, "Item")
            }
        };
        var result = _validator.TestValidate(command);
        result.IsValid.Should().BeFalse();
    }
}
