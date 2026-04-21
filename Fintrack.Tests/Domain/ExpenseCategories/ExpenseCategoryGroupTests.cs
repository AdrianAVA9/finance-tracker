using Fintrack.Server.Domain.ExpenseCategories;
using Fintrack.Server.Domain.ExpenseCategories.Events;
using FluentAssertions;

namespace Fintrack.Tests.Domain.ExpenseCategories;

public sealed class ExpenseCategoryGroupTests
{
    [Fact]
    public void CreateForUser_Should_Succeed_And_RaiseCreatedEvent()
    {
        var result = ExpenseCategoryGroup.CreateForUser("My Group", "Desc", "user-1");

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("My Group");
        result.Value.Description.Should().Be("Desc");
        result.Value.UserId.Should().Be("user-1");
        result.Value.IsEditable.Should().BeTrue();

        result.Value.GetDomainEvents().Should().ContainSingle()
            .Which.Should().BeOfType<ExpenseCategoryGroupCreatedDomainEvent>()
            .Which.ExpenseCategoryGroupId.Should().Be(result.Value.Id);
    }

    [Fact]
    public void CreateForSystem_Should_Be_NotEditable()
    {
        var result = ExpenseCategoryGroup.CreateForSystem("System", null);

        result.IsSuccess.Should().BeTrue();
        result.Value.UserId.Should().BeNull();
        result.Value.IsEditable.Should().BeFalse();
    }

    [Fact]
    public void CreateForUser_Should_Fail_When_NameInvalid()
    {
        var result = ExpenseCategoryGroup.CreateForUser("  ", null, "u1");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ExpenseCategoryGroupErrors.NameRequired);
    }

    [Fact]
    public void Update_Should_Succeed_When_Editable()
    {
        var group = ExpenseCategoryGroup.CreateForUser("A", null, "u1").Value;
        group.ClearDomainEvents();

        var update = group.Update("B", "new desc");

        update.IsSuccess.Should().BeTrue();
        group.Name.Should().Be("B");
        group.Description.Should().Be("new desc");
        group.GetDomainEvents().Should().ContainSingle()
            .Which.Should().BeOfType<ExpenseCategoryGroupUpdatedDomainEvent>()
            .Which.ExpenseCategoryGroupId.Should().Be(group.Id);
    }

    [Fact]
    public void Update_Should_Fail_When_NotEditable()
    {
        var group = ExpenseCategoryGroup.CreateForSystem("Sys", null).Value;

        var update = group.Update("X", null);

        update.IsFailure.Should().BeTrue();
        update.Error.Should().Be(ExpenseCategoryGroupErrors.NotEditable);
    }

    [Fact]
    public void RegisterDeletion_Should_Succeed_When_Editable()
    {
        var group = ExpenseCategoryGroup.CreateForUser("Del", null, "u1").Value;
        group.ClearDomainEvents();

        var del = group.RegisterDeletion();

        del.IsSuccess.Should().BeTrue();
        group.GetDomainEvents().Should().ContainSingle()
            .Which.Should().BeOfType<ExpenseCategoryGroupDeletedDomainEvent>()
            .Which.ExpenseCategoryGroupId.Should().Be(group.Id);
    }

    [Fact]
    public void RegisterDeletion_Should_Fail_When_NotEditable()
    {
        var group = ExpenseCategoryGroup.CreateForSystem("Sys", null).Value;

        var del = group.RegisterDeletion();

        del.IsFailure.Should().BeTrue();
        del.Error.Should().Be(ExpenseCategoryGroupErrors.NotEditable);
    }
}
