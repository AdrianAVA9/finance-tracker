using System;
using Fintrack.Server.Domain.ExpenseCategories;
using Fintrack.Server.Domain.ExpenseCategories.Events;
using FluentAssertions;

namespace Fintrack.Tests.Domain.ExpenseCategories;

public sealed class ExpenseCategoryTests
{
    [Fact]
    public void Create_Should_Succeed_When_ParametersValid()
    {
        var groupId = Guid.Parse("00000000-0000-0000-0000-000000000002");
        var result = ExpenseCategory.Create(
            "Groceries",
            "Weekly shop",
            "cart",
            "#111111",
            groupId: groupId,
            userId: "user-1",
            isEditable: true);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Groceries");
        result.Value.Description.Should().Be("Weekly shop");
        result.Value.Icon.Should().Be("cart");
        result.Value.Color.Should().Be("#111111");
        result.Value.GroupId.Should().Be(groupId);
        result.Value.UserId.Should().Be("user-1");
        result.Value.IsEditable.Should().BeTrue();
    }

    [Fact]
    public void Create_Should_RaiseCreatedEvent_When_Success()
    {
        var result = ExpenseCategory.Create("Cat", null, null, null, null, "u1", true);

        result.IsSuccess.Should().BeTrue();
        var evt = result.Value.GetDomainEvents().Should().ContainSingle().Which;
        evt.Should().BeOfType<ExpenseCategoryCreatedDomainEvent>()
            .Which.ExpenseCategoryId.Should().Be(result.Value.Id);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Should_Fail_When_NameInvalid(string name)
    {
        var result = ExpenseCategory.Create(name, null, null, null, null, null, true);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ExpenseCategoryErrors.NameRequired);
    }

    [Fact]
    public void Create_Should_Fail_When_NameTooLong()
    {
        var name = new string('x', 201);

        var result = ExpenseCategory.Create(name, null, null, null, null, null, true);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ExpenseCategoryErrors.NameTooLong);
    }

    [Fact]
    public void Update_Should_Succeed_And_RaiseEvent_When_Editable()
    {
        var g1 = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var g2 = Guid.Parse("00000000-0000-0000-0000-000000000002");
        var category = ExpenseCategory.Create("A", null, null, null, g1, "u1", true).Value;
        category.ClearDomainEvents();

        var update = category.Update("B", "d", "i", "#fff", g2);

        update.IsSuccess.Should().BeTrue();
        category.Name.Should().Be("B");
        category.Description.Should().Be("d");
        category.Icon.Should().Be("i");
        category.Color.Should().Be("#fff");
        category.GroupId.Should().Be(g2);
        category.GetDomainEvents().Should().ContainSingle()
            .Which.Should().BeOfType<ExpenseCategoryUpdatedDomainEvent>()
            .Which.ExpenseCategoryId.Should().Be(category.Id);
    }

    [Fact]
    public void Update_Should_Fail_When_NotEditable()
    {
        var category = ExpenseCategory.Create("Sys", null, null, null, null, null, isEditable: false).Value;

        var update = category.Update("X", null, null, null, null);

        update.IsFailure.Should().BeTrue();
        update.Error.Should().Be(ExpenseCategoryErrors.NotEditable);
    }

    [Fact]
    public void RegisterDeletion_Should_Succeed_And_RaiseEvent_When_Editable()
    {
        var category = ExpenseCategory.Create("Del", null, null, null, null, "u1", true).Value;
        category.ClearDomainEvents();

        var del = category.RegisterDeletion();

        del.IsSuccess.Should().BeTrue();
        category.GetDomainEvents().Should().ContainSingle()
            .Which.Should().BeOfType<ExpenseCategoryDeletedDomainEvent>()
            .Which.ExpenseCategoryId.Should().Be(category.Id);
    }

    [Fact]
    public void RegisterDeletion_Should_Fail_When_NotEditable()
    {
        var category = ExpenseCategory.Create("Sys", null, null, null, null, null, isEditable: false).Value;

        var del = category.RegisterDeletion();

        del.IsFailure.Should().BeTrue();
        del.Error.Should().Be(ExpenseCategoryErrors.NotDeletable);
    }
}
