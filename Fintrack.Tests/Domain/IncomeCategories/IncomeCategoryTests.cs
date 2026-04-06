using Fintrack.Server.Domain.IncomeCategories;
using Fintrack.Server.Domain.IncomeCategories.Events;
using FluentAssertions;

namespace Fintrack.Tests.Domain.IncomeCategories;

public sealed class IncomeCategoryTests
{
    [Fact]
    public void Create_Should_Succeed_When_ParametersValid()
    {
        var result = IncomeCategory.Create(
            "Salario",
            "payments",
            "#10B981",
            userId: "user-1",
            isEditable: true);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Salario");
        result.Value.Icon.Should().Be("payments");
        result.Value.Color.Should().Be("#10B981");
        result.Value.UserId.Should().Be("user-1");
        result.Value.IsEditable.Should().BeTrue();
    }

    [Fact]
    public void Create_Should_RaiseCreatedEvent_When_Success()
    {
        var result = IncomeCategory.Create("Cat", null, null, null, true);

        result.IsSuccess.Should().BeTrue();
        var evt = result.Value.GetDomainEvents().Should().ContainSingle().Which;
        evt.Should().BeOfType<IncomeCategoryCreatedDomainEvent>()
            .Which.IncomeCategoryId.Should().Be(result.Value.Id);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Should_Fail_When_NameInvalid(string name)
    {
        var result = IncomeCategory.Create(name, null, null, null, true);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(IncomeCategoryErrors.NameRequired);
    }

    [Fact]
    public void Create_Should_Fail_When_NameTooLong()
    {
        var name = new string('x', 201);

        var result = IncomeCategory.Create(name, null, null, null, true);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(IncomeCategoryErrors.NameTooLong);
    }

    [Fact]
    public void Update_Should_Succeed_And_RaiseEvent_When_Editable()
    {
        var category = IncomeCategory.Create("A", "i1", "#000", "u1", true).Value;
        category.ClearDomainEvents();

        var update = category.Update("B", "i2", "#fff");

        update.IsSuccess.Should().BeTrue();
        category.Name.Should().Be("B");
        category.Icon.Should().Be("i2");
        category.Color.Should().Be("#fff");
        category.GetDomainEvents().Should().ContainSingle()
            .Which.Should().BeOfType<IncomeCategoryUpdatedDomainEvent>()
            .Which.IncomeCategoryId.Should().Be(category.Id);
    }

    [Fact]
    public void Update_Should_Fail_When_NotEditable()
    {
        var category = IncomeCategory.Create("Sys", null, null, null, isEditable: false).Value;

        var update = category.Update("X", null, null);

        update.IsFailure.Should().BeTrue();
        update.Error.Should().Be(IncomeCategoryErrors.NotEditable);
    }

    [Fact]
    public void RegisterDeletion_Should_Fail_When_NotEditable()
    {
        var category = IncomeCategory.Create("Sys", null, null, null, isEditable: false).Value;

        var del = category.RegisterDeletion();

        del.IsFailure.Should().BeTrue();
        del.Error.Should().Be(IncomeCategoryErrors.NotDeletable);
    }

    [Fact]
    public void RegisterDeletion_Should_Succeed_And_RaiseEvent_When_Editable()
    {
        var category = IncomeCategory.Create("Mine", null, null, "u1", true).Value;
        category.ClearDomainEvents();

        var del = category.RegisterDeletion();

        del.IsSuccess.Should().BeTrue();
        category.GetDomainEvents().Should().ContainSingle()
            .Which.Should().BeOfType<IncomeCategoryDeletedDomainEvent>()
            .Which.IncomeCategoryId.Should().Be(category.Id);
    }
}
