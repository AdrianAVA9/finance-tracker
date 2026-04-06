using Fintrack.Server.Application.Incomes.Commands.DeleteIncome;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Incomes;
using Fintrack.Tests.Abstractions;
using FluentAssertions;
using NSubstitute;

namespace Fintrack.Tests.Application.Incomes.Commands.DeleteIncome;

public sealed class DeleteIncomeCommandHandlerTests : BaseUnitTest
{
    private readonly IIncomeRepository _incomeRepository = Mock<IIncomeRepository>();
    private readonly IUnitOfWork _unitOfWork = Mock<IUnitOfWork>();

    private DeleteIncomeCommandHandler CreateHandler() =>
        new(_incomeRepository, _unitOfWork);

    [Fact]
    public async Task Handle_Should_ReturnNotFound_When_IncomeMissing()
    {
        var id = Guid.NewGuid();
        _incomeRepository.GetByIdAsync(id, "user-1", CancellationToken).Returns((Income?)null);

        var result = await CreateHandler().Handle(new DeleteIncomeCommand(id, "user-1"), CancellationToken);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(IncomeErrors.NotFound);
        _incomeRepository.DidNotReceive().Remove(Arg.Any<Income>());
    }

    [Fact]
    public async Task Handle_Should_Remove_When_Found()
    {
        var income = Income.Create(
            "user-1",
            "S",
            10m,
            Guid.NewGuid(),
            DateTime.UtcNow.Date,
            null).Value;

        _incomeRepository.GetByIdAsync(income.Id, "user-1", CancellationToken).Returns(income);
        _unitOfWork.SaveChangesAsync(CancellationToken).Returns(1);

        var result = await CreateHandler().Handle(new DeleteIncomeCommand(income.Id, "user-1"), CancellationToken);

        result.IsSuccess.Should().BeTrue();
        _incomeRepository.Received(1).Remove(income);
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken);
    }
}
