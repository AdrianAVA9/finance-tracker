using System;
using Fintrack.Server.Application.ExpenseCategoryGroups.Commands.CreateExpenseCategoryGroup;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.ExpenseCategories;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Fintrack.Tests.Application.ExpenseCategoryGroups.Commands.CreateExpenseCategoryGroup;

public sealed class CreateExpenseCategoryGroupCommandHandlerTests
{
    private readonly IExpenseCategoryGroupRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CreateExpenseCategoryGroupCommandHandler _handler;

    public CreateExpenseCategoryGroupCommandHandlerTests()
    {
        _repository = Substitute.For<IExpenseCategoryGroupRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();

        _handler = new CreateExpenseCategoryGroupCommandHandler(
            _repository,
            _unitOfWork);
    }

    [Fact]
    public async Task Handle_Should_AddEntityAndReturnId_When_ValidRequest()
    {
        var command = new CreateExpenseCategoryGroupCommand(
            "Custom Group",
            "Description",
            "user-123");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Guid.Empty);

        _repository
            .Received(1)
            .Add(Arg.Is<ExpenseCategoryGroup>(g =>
                g.Name == command.Name &&
                g.Description == command.Description &&
                g.UserId == command.UserId &&
                g.IsEditable));

        await _unitOfWork
            .Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
