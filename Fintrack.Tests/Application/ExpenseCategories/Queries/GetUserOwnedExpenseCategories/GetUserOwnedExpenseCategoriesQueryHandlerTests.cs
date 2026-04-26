using Fintrack.Server.Application.ExpenseCategories.Queries.GetUserOwnedExpenseCategories;
using Fintrack.Server.Domain.ExpenseCategories;
using FluentAssertions;
using NSubstitute;

namespace Fintrack.Tests.Application.ExpenseCategories.Queries.GetUserOwnedExpenseCategories;

public sealed class GetUserOwnedExpenseCategoriesQueryHandlerTests
{
    private readonly IExpenseCategoryRepository _repository = Substitute.For<IExpenseCategoryRepository>();

    private GetUserOwnedExpenseCategoriesQueryHandler CreateSut() => new(_repository);

    [Fact]
    public async Task Handle_Should_ReturnUserOwnedCategories()
    {
        var group = ExpenseCategoryGroup.CreateForSystem("G", null).Value;
        var cat = ExpenseCategory.Create("Mine", null, "icon", "#000", group.Id, "user-1", isEditable: true).Value;
        _repository.GetUserOwnedByUserIdAsync("user-1", Arg.Any<CancellationToken>())
            .Returns(new List<ExpenseCategory> { cat });

        var sut = CreateSut();
        var result = await sut.Handle(new GetUserOwnedExpenseCategoriesQuery("user-1"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value![0].Name.Should().Be("Mine");
        result.Value[0].UserId.Should().Be("user-1");
    }
}
