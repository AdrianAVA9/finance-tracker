using Fintrack.Server.Application.IncomeCategories.Queries.GetUserOwnedIncomeCategories;
using Fintrack.Server.Domain.IncomeCategories;
using FluentAssertions;
using NSubstitute;

namespace Fintrack.Tests.Application.IncomeCategories.Queries.GetUserOwnedIncomeCategories;

public sealed class GetUserOwnedIncomeCategoriesQueryHandlerTests
{
    private readonly IIncomeCategoryRepository _repository = Substitute.For<IIncomeCategoryRepository>();

    private GetUserOwnedIncomeCategoriesQueryHandler CreateSut() => new(_repository);

    [Fact]
    public async Task Handle_Should_ReturnOnlyUserOwnedDtos_When_RepositoryReturnsCategories()
    {
        var userCat = IncomeCategory.Create("My income", "work", "#fff", "user-1", isEditable: true).Value;
        _repository.GetUserOwnedByUserIdAsync("user-1", Arg.Any<CancellationToken>())
            .Returns(new List<IncomeCategory> { userCat });

        var sut = CreateSut();
        var result = await sut.Handle(new GetUserOwnedIncomeCategoriesQuery("user-1"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value![0].Name.Should().Be("My income");
        result.Value[0].IsEditable.Should().BeTrue();
    }
}
