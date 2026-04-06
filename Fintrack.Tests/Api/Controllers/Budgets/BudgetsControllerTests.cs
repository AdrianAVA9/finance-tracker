using System.Security.Claims;
using Fintrack.Server.Api.Controllers.Budgets;
using Fintrack.Server.Application.Budgets.Commands.CopyPreviousMonthBudgets;
using Fintrack.Server.Application.Budgets.Commands.DeleteBudget;
using Fintrack.Server.Application.Budgets.Commands.UpsertBudgets;
using Fintrack.Server.Application.Budgets.Queries.GetBudgetDetails;
using Fintrack.Server.Application.Budgets.Queries.GetBudgets;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Budgets;
using Fintrack.Tests.Abstractions;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace Fintrack.Tests.Api.Controllers.Budgets;

/// <summary>
/// Controller-focused unit tests: MediatR is mocked; no HTTP pipeline, EF, or authorization filters.
/// Permission and routing coverage stays in <c>Fintrack.IntegrationTests</c>.
/// </summary>
public sealed class BudgetsControllerTests : BaseUnitTest
{
    private readonly ISender _sender = Mock<ISender>();

    private static BudgetsController CreateController(ISender sender, string? userId = null)
    {
        var controller = new BudgetsController(sender);
        var httpContext = new DefaultHttpContext();
        if (userId is not null)
        {
            var identity = new ClaimsIdentity(
                [new Claim(ClaimTypes.NameIdentifier, userId)],
                authenticationType: "Test");
            httpContext.User = new ClaimsPrincipal(identity);
        }

        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        return controller;
    }

    [Fact]
    public async Task Get_Should_ReturnUnauthorized_When_UserIdMissing()
    {
        var controller = CreateController(_sender, userId: null);

        var result = await controller.Get(month: 3, year: 2024);

        result.Should().BeOfType<UnauthorizedResult>();
        await _sender.DidNotReceive().Send(Arg.Any<IRequest<Result<BudgetListDto>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Get_Should_SendQuery_And_ReturnOk_When_Success()
    {
        var userId = "user-1";
        var controller = CreateController(_sender, userId);
        var payload = new BudgetListDto(Budgets: [], MonthlyIncome: 0m);
        _sender
            .Send(Arg.Any<GetBudgetsQuery>(), CancellationToken)
            .Returns(Result.Success(payload));

        var result = await controller.Get(month: 3, year: 2024);

        await _sender.Received(1).Send(
            Arg.Is<GetBudgetsQuery>(q => q.UserId == userId && q.Month == 3 && q.Year == 2024),
            CancellationToken);
        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeSameAs(payload);
    }

    [Fact]
    public async Task Upsert_Should_ReturnUnauthorized_When_UserIdMissing()
    {
        var controller = CreateController(_sender, userId: null);
        var request = new UpsertBudgetsRequest(1, 2024, []);

        var result = await controller.Upsert(request);

        result.Should().BeOfType<UnauthorizedResult>();
        await _sender.DidNotReceive().Send(Arg.Any<IRequest<Result>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Upsert_Should_SendCommand_And_ReturnOk_When_Success()
    {
        var userId = "user-1";
        var controller = CreateController(_sender, userId);
        var entries = new List<BudgetEntryDto> { new(Guid.NewGuid(), 100m) };
        var request = new UpsertBudgetsRequest(2, 2025, entries);
        _sender.Send(Arg.Any<UpsertBudgetsCommand>(), CancellationToken).Returns(Result.Success());

        var result = await controller.Upsert(request);

        await _sender.Received(1).Send(
            Arg.Is<UpsertBudgetsCommand>(c =>
                c.UserId == userId && c.Month == 2 && c.Year == 2025 && c.Budgets.SequenceEqual(entries)),
            CancellationToken);
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task CopyPrevious_Should_SendCommand_With_TargetMonthYear()
    {
        var userId = "user-1";
        var controller = CreateController(_sender, userId);
        _sender.Send(Arg.Any<CopyPreviousMonthBudgetsCommand>(), CancellationToken).Returns(Result.Success());

        var result = await controller.CopyPrevious(new CopyPreviousRequest(Month: 4, Year: 2026));

        await _sender.Received(1).Send(
            Arg.Is<CopyPreviousMonthBudgetsCommand>(c =>
                c.UserId == userId && c.TargetMonth == 4 && c.TargetYear == 2026),
            CancellationToken);
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task Delete_Should_ReturnNotFound_When_BudgetMissing()
    {
        var userId = "user-1";
        var id = Guid.NewGuid();
        var controller = CreateController(_sender, userId);
        _sender
            .Send(Arg.Any<DeleteBudgetCommand>(), CancellationToken)
            .Returns(Result.Failure(BudgetErrors.NotFound));

        var result = await controller.Delete(id);

        await _sender.Received(1).Send(
            Arg.Is<DeleteBudgetCommand>(c => c.Id == id && c.UserId == userId),
            CancellationToken);
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetById_Should_ReturnOk_When_Success()
    {
        var userId = "user-1";
        var budgetId = Guid.NewGuid();
        var controller = CreateController(_sender, userId);
        var dto = new BudgetDetailsDto(
            budgetId,
            CategoryName: "Food",
            LimitAmount: 400m,
            MonthlyHistory: []);
        _sender
            .Send(Arg.Any<GetBudgetDetailsQuery>(), CancellationToken)
            .Returns(Result.Success(dto));

        var result = await controller.GetById(budgetId, year: 2024, month: 6);

        await _sender.Received(1).Send(
            Arg.Is<GetBudgetDetailsQuery>(q =>
                q.BudgetId == budgetId && q.UserId == userId && q.Month == 6 && q.Year == 2024),
            CancellationToken);
        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeSameAs(dto);
    }
}
