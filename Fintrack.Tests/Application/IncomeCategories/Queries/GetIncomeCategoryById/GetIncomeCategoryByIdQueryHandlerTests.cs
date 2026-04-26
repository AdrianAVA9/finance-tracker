using System;
using Fintrack.Server.Application.IncomeCategories.Queries.GetIncomeCategories;
using Fintrack.Server.Application.IncomeCategories.Queries.GetIncomeCategoryById;
using Fintrack.Server.Domain.IncomeCategories;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Fintrack.Tests.Application.IncomeCategories.Queries.GetIncomeCategoryById;

public sealed class GetIncomeCategoryByIdQueryHandlerTests
{
    private readonly IIncomeCategoryRepository _repo;
    private readonly GetIncomeCategoryByIdQueryHandler _handler;

    public GetIncomeCategoryByIdQueryHandlerTests()
    {
        _repo = Substitute.For<IIncomeCategoryRepository>();
        _handler = new GetIncomeCategoryByIdQueryHandler(_repo);
    }

    [Fact]
    public async Task Handle_Should_ReturnDto_When_UserCanAccess()
    {
        var userId = "u-1";
        var create = IncomeCategory.Create("Cat", "x", "#111", userId, isEditable: true);
        create.IsSuccess.Should().BeTrue();
        var cat = create.Value;
        _repo.GetByIdAsync(cat.Id, Arg.Any<CancellationToken>())
            .Returns(cat);

        var result = await _handler.Handle(
            new GetIncomeCategoryByIdQuery(cat.Id, userId),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(new IncomeCategoryDto(
            create.Value.Id,
            "Cat",
            "x",
            "#111",
            true));
    }
}
