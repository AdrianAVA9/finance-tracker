using System;
using Fintrack.Server.Application.ExpenseCategories.Commands.CreateExpenseCategory;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.ExpenseCategories;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Fintrack.Tests.Application.ExpenseCategories.Commands.CreateExpenseCategory;

public sealed class CreateExpenseCategoryCommandHandlerTests
{
    private readonly IExpenseCategoryRepository _expenseCategoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CreateExpenseCategoryCommandHandler _handler;

    public CreateExpenseCategoryCommandHandlerTests()
    {
        _expenseCategoryRepository = Substitute.For<IExpenseCategoryRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();

        _handler = new CreateExpenseCategoryCommandHandler(
            _expenseCategoryRepository,
            _unitOfWork);
    }

    [Fact]
    public async Task Handle_Should_AddEntityAndCallSaveChanges_When_ValidRequest()
    {
        var command = new CreateExpenseCategoryCommand(
            "Test Category",
            "Test Description",
            "icon-test",
            "#ffffff",
            Guid.NewGuid(),
            "user-123");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        _expenseCategoryRepository
            .Received(1)
            .Add(Arg.Is<ExpenseCategory>(e =>
                e.Id == result.Value &&
                e.Name == command.Name &&
                e.Description == command.Description &&
                e.UserId == command.UserId &&
                e.IsEditable == true &&
                e.GroupId == command.GroupId &&
                e.Icon == command.Icon &&
                e.Color == command.Color));

        await _unitOfWork
            .Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
