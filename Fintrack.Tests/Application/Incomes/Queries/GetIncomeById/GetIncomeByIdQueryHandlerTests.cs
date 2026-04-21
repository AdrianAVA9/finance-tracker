using Fintrack.Server.Application.Incomes.Queries.GetIncomeById;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Incomes;
using Fintrack.Tests.Abstractions;
using FluentAssertions;
using NSubstitute;

namespace Fintrack.Tests.Application.Incomes.Queries.GetIncomeById;

public sealed class GetIncomeByIdQueryHandlerTests : BaseUnitTest
{
    private readonly IIncomeRepository _incomeRepository = Mock<IIncomeRepository>();
    private readonly IRecurringIncomeRepository _recurringIncomeRepository = Mock<IRecurringIncomeRepository>();

    private GetIncomeByIdQueryHandler CreateHandler() =>
        new(_incomeRepository, _recurringIncomeRepository);

    [Fact]
    public async Task Handle_Should_ReturnNotFound_When_IncomeMissing()
    {
        var id = Guid.NewGuid();
        _incomeRepository.GetByIdAsNoTrackingAsync(id, "user-1", CancellationToken).Returns((Income?)null);

        var result = await CreateHandler().Handle(new GetIncomeByIdQuery(id, "user-1"), CancellationToken);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(IncomeErrors.NotFound);
    }

    [Fact]
    public async Task Handle_Should_ReturnDto_When_Found()
    {
        var income = Income.Create(
            "user-1",
            "Job",
            300m,
            Guid.NewGuid(),
            DateTime.UtcNow.Date,
            "n").Value;

        _incomeRepository.GetByIdAsNoTrackingAsync(income.Id, "user-1", CancellationToken).Returns(income);
        _recurringIncomeRepository
            .FindActiveMatchingTemplateAsync("user-1", "Job", income.CategoryId, 300m, CancellationToken)
            .Returns((RecurringIncome?)null);

        var result = await CreateHandler().Handle(new GetIncomeByIdQuery(income.Id, "user-1"), CancellationToken);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(income.Id);
        result.Value.Source.Should().Be("Job");
        result.Value.IsRecurring.Should().BeFalse();
    }
}
