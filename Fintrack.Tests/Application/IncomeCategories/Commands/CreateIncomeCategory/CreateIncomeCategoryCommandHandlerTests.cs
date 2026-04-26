using Fintrack.Server.Application.IncomeCategories.Commands.CreateIncomeCategory;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.IncomeCategories;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Fintrack.Tests.Application.IncomeCategories.Commands.CreateIncomeCategory;

public sealed class CreateIncomeCategoryCommandHandlerTests
{
    private readonly IIncomeCategoryRepository _incomeCategoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CreateIncomeCategoryCommandHandler _handler;

    public CreateIncomeCategoryCommandHandlerTests()
    {
        _incomeCategoryRepository = Substitute.For<IIncomeCategoryRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();

        _handler = new CreateIncomeCategoryCommandHandler(
            _incomeCategoryRepository,
            _unitOfWork);
    }

    [Fact]
    public async Task Handle_Should_AddEntityAndCallSaveChanges_When_ValidRequest()
    {
        var command = new CreateIncomeCategoryCommand(
            "Freelance",
            "laptop",
            "#05e699",
            "user-123");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        _incomeCategoryRepository
            .Received(1)
            .Add(Arg.Is<IncomeCategory>(e =>
                e.Id == result.Value &&
                e.Name == command.Name &&
                e.UserId == command.UserId &&
                e.IsEditable &&
                e.Icon == command.Icon &&
                e.Color == command.Color));

        await _unitOfWork
            .Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
