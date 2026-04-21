using Fintrack.Server.Application.IncomeCategories.Queries.GetIncomeCategories;
using Fintrack.Server.Domain.IncomeCategories;
using FluentAssertions;
using NSubstitute;

namespace Fintrack.Tests.Application.IncomeCategories.Queries.GetIncomeCategories;

public sealed class GetIncomeCategoriesQueryHandlerTests
{
    private readonly IIncomeCategoryRepository _repository = Substitute.For<IIncomeCategoryRepository>();

    private GetIncomeCategoriesQueryHandler CreateSut() => new(_repository);

    [Fact]
    public async Task Handle_Should_ReturnOrderedDtos_When_RepositoryReturnsCategories()
    {
        var catA = IncomeCategory.Create("A", "icon", "#fff", null, false).Value;
        var catB = IncomeCategory.Create("B", null, null, null, false).Value;
        _repository.GetAllByUserIdAsync("user-1", Arg.Any<CancellationToken>())
            .Returns(new List<IncomeCategory> { catA, catB });

        var sut = CreateSut();
        var result = await sut.Handle(new GetIncomeCategoriesQuery("user-1"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value![0].Name.Should().Be("A");
        result.Value[0].Icon.Should().Be("icon");
        result.Value[0].Color.Should().Be("#fff");
        result.Value[1].Name.Should().Be("B");
        result.Value[0].IsEditable.Should().BeFalse();
    }
}
