using System;
using Fintrack.Server.Application.IncomeCategories.Commands.UpdateIncomeCategory;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.IncomeCategories;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Fintrack.Tests.Application.IncomeCategories.Commands.UpdateIncomeCategory;

public sealed class UpdateIncomeCategoryCommandHandlerTests
{
    private readonly IIncomeCategoryRepository _incomeCategoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UpdateIncomeCategoryCommandHandler _handler;

    public UpdateIncomeCategoryCommandHandlerTests()
    {
        _incomeCategoryRepository = Substitute.For<IIncomeCategoryRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();

        _handler = new UpdateIncomeCategoryCommandHandler(
            _incomeCategoryRepository,
            _unitOfWork);
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFound_When_CategoryDoesNotExist()
    {
        var id = Guid.NewGuid();
        _incomeCategoryRepository.GetByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns((IncomeCategory?)null);

        var result = await _handler.Handle(
            new UpdateIncomeCategoryCommand(id, "A", "i", "#fff", "u1"),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(IncomeCategoryErrors.NotFound);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_CategoryIsNotEditable()
    {
        var systemCat = IncomeCategory.Create("Sistema", null, null, null, isEditable: false);
        systemCat.IsSuccess.Should().BeTrue();
        var cat = systemCat.Value;
        _incomeCategoryRepository.GetByIdAsync(cat.Id, Arg.Any<CancellationToken>())
            .Returns(cat);

        var result = await _handler.Handle(
            new UpdateIncomeCategoryCommand(cat.Id, "Nuevo", null, null, "any"),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(IncomeCategoryErrors.NotEditable);
    }

    [Fact]
    public async Task Handle_Should_Persist_When_UserOwnedEditable()
    {
        var userId = "user-99";
        var create = IncomeCategory.Create("A", "w", null, userId, isEditable: true);
        create.IsSuccess.Should().BeTrue();
        var cat = create.Value;

        _incomeCategoryRepository.GetByIdAsync(cat.Id, Arg.Any<CancellationToken>())
            .Returns(cat);

        var result = await _handler.Handle(
            new UpdateIncomeCategoryCommand(cat.Id, "B", "work", "#abc", userId),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _incomeCategoryRepository.Received(1).Update(cat);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        cat.Name.Should().Be("B");
    }
}
