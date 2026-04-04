using FluentAssertions;
using NSubstitute;
using Fintrack.Server.Application.ExpenseCategories.Commands;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.ExpenseCategories;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Budgets;
using Fintrack.Server.Domain.Enums;
using Fintrack.Server.Domain.Exceptions;
using Fintrack.Server.Domain.ExpenseCategories;
using Fintrack.Server.Domain.Expenses;
using Fintrack.Server.Domain.Incomes;
using Fintrack.Server.Domain.Invoices;
using Fintrack.Server.Domain.SavingsGoals;
using Fintrack.Server.Domain.Users;

namespace Fintrack.Tests.Application.ExpenseCategories.Commands
{
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
            // Arrange
            var command = new CreateExpenseCategoryCommand(
                "Test Category",
                "Test Description",
                "icon-test",
                "#ffffff",
                1,
                "user-123");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _expenseCategoryRepository
                .Received(1)
                .Add(Arg.Is<ExpenseCategory>(e =>
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
}
