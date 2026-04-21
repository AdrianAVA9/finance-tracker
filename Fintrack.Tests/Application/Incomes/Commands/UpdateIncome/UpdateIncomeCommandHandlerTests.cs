using Fintrack.Server.Application.Incomes.Commands.UpdateIncome;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Incomes;
using Fintrack.Tests.Abstractions;
using FluentAssertions;
using NSubstitute;

namespace Fintrack.Tests.Application.Incomes.Commands.UpdateIncome;

public sealed class UpdateIncomeCommandHandlerTests : BaseUnitTest
{
    private readonly IIncomeRepository _incomeRepository = Mock<IIncomeRepository>();
    private readonly IRecurringIncomeRepository _recurringIncomeRepository = Mock<IRecurringIncomeRepository>();
    private readonly IUnitOfWork _unitOfWork = Mock<IUnitOfWork>();

    private UpdateIncomeCommandHandler CreateHandler() =>
        new(_incomeRepository, _recurringIncomeRepository, _unitOfWork);

    [Fact]
    public async Task Handle_Should_ReturnNotFound_When_IncomeMissing()
    {
        var id = Guid.NewGuid();
        _incomeRepository.GetByIdAsync(id, "user-1", CancellationToken).Returns((Income?)null);

        var command = new UpdateIncomeCommand(
            id,
            "user-1",
            "S",
            10m,
            Guid.NewGuid(),
            DateTime.UtcNow.Date,
            null,
            false,
            null,
            null);

        var result = await CreateHandler().Handle(command, CancellationToken);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(IncomeErrors.NotFound);
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_Update_When_Found()
    {
        var income = Income.Create(
            "user-1",
            "Old",
            100m,
            Guid.NewGuid(),
            DateTime.UtcNow.Date,
            null).Value;

        _incomeRepository.GetByIdAsync(income.Id, "user-1", CancellationToken).Returns(income);
        _recurringIncomeRepository
            .FindActiveMatchingTemplateAsync("user-1", "Old", income.CategoryId, 100m, CancellationToken)
            .Returns((RecurringIncome?)null);
        _unitOfWork.SaveChangesAsync(CancellationToken).Returns(1);

        var newCategory = Guid.NewGuid();
        var command = new UpdateIncomeCommand(
            income.Id,
            "user-1",
            "New",
            200m,
            newCategory,
            DateTime.UtcNow.Date,
            "note",
            false,
            null,
            null);

        var result = await CreateHandler().Handle(command, CancellationToken);

        result.IsSuccess.Should().BeTrue();
        income.Source.Should().Be("New");
        income.Amount.Should().Be(200m);
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken);
    }
}
