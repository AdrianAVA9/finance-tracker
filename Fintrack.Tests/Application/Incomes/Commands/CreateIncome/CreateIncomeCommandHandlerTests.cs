using Fintrack.Server.Application.Incomes.Commands.CreateIncome;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Enums;
using Fintrack.Server.Domain.Incomes;
using Fintrack.Tests.Abstractions;
using FluentAssertions;
using NSubstitute;

namespace Fintrack.Tests.Application.Incomes.Commands.CreateIncome;

public sealed class CreateIncomeCommandHandlerTests : BaseUnitTest
{
    private readonly IIncomeRepository _incomeRepository = Mock<IIncomeRepository>();
    private readonly IRecurringIncomeRepository _recurringIncomeRepository = Mock<IRecurringIncomeRepository>();
    private readonly IUnitOfWork _unitOfWork = Mock<IUnitOfWork>();

    private CreateIncomeCommandHandler CreateHandler() =>
        new(_incomeRepository, _recurringIncomeRepository, _unitOfWork);

    [Fact]
    public async Task Handle_Should_AddIncomeAndReturnId_When_Valid()
    {
        _unitOfWork.SaveChangesAsync(CancellationToken).Returns(1);

        var command = new CreateIncomeCommand(
            UserId: "user-1",
            Source: "Salary",
            Amount: 100m,
            CategoryId: Guid.NewGuid(),
            Date: DateTime.UtcNow.Date,
            Notes: null,
            IsRecurring: false);

        var result = await CreateHandler().Handle(command, CancellationToken);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        _incomeRepository.Received(1).Add(Arg.Is<Income>(i =>
            i.UserId == "user-1" && i.Source == "Salary" && i.Amount == 100m));
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken);
        _recurringIncomeRepository.DidNotReceive().Add(Arg.Any<RecurringIncome>());
    }

    [Fact]
    public async Task Handle_Should_AddRecurringTemplate_When_IsRecurring()
    {
        _unitOfWork.SaveChangesAsync(CancellationToken).Returns(1);

        var command = new CreateIncomeCommand(
            UserId: "user-1",
            Source: "Rent",
            Amount: 500m,
            CategoryId: Guid.NewGuid(),
            Date: DateTime.UtcNow.Date,
            Notes: null,
            IsRecurring: true,
            Frequency: RecurringFrequency.Monthly,
            NextDate: null);

        var result = await CreateHandler().Handle(command, CancellationToken);

        result.IsSuccess.Should().BeTrue();
        _recurringIncomeRepository.Received(1).Add(Arg.Is<RecurringIncome>(r =>
            r.UserId == "user-1" &&
            r.Frequency == RecurringFrequency.Monthly &&
            r.IsActive));
    }
}
