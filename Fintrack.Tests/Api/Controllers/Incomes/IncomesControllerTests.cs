using System.Security.Claims;
using Fintrack.Server.Api.Controllers.Incomes;
using Fintrack.Server.Application.IncomeCategories.Queries.GetIncomeCategories;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.IncomeCategories;
using Fintrack.Tests.Abstractions;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace Fintrack.Tests.Api.Controllers.Incomes;

/// <summary>
/// Controller-focused unit tests for income category endpoints; MediatR is mocked.
/// </summary>
public sealed class IncomesControllerTests : BaseUnitTest
{
    private readonly ISender _sender = Mock<ISender>();

    private static IncomesController CreateController(ISender sender, string? userId = null)
    {
        var controller = new IncomesController(sender);
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
    public async Task GetCategories_Should_ReturnUnauthorized_When_UserIdMissing()
    {
        var controller = CreateController(_sender, userId: null);

        var result = await controller.GetCategories(CancellationToken);

        result.Should().BeOfType<UnauthorizedResult>();
        await _sender.DidNotReceive().Send(
            Arg.Any<GetIncomeCategoriesQuery>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetCategories_Should_SendQuery_And_ReturnOk_When_Success()
    {
        const string userId = "user-1";
        var controller = CreateController(_sender, userId);
        var catId = Guid.Parse("00000000-0000-0000-0000-0000000000aa");
        IReadOnlyList<IncomeCategoryDto> payload =
        [
            new IncomeCategoryDto(catId, "Salario", "payments", "#10B981", false)
        ];
        _sender
            .Send(Arg.Any<GetIncomeCategoriesQuery>(), CancellationToken)
            .Returns(Result.Success(payload));

        var result = await controller.GetCategories(CancellationToken);

        await _sender.Received(1).Send(
            Arg.Is<GetIncomeCategoriesQuery>(q => q.UserId == userId),
            CancellationToken);
        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeSameAs(payload);
    }

    [Fact]
    public async Task GetCategories_Should_ReturnNotFound_When_HandlerReturnsNotFound()
    {
        var controller = CreateController(_sender, "user-1");
        _sender
            .Send(Arg.Any<GetIncomeCategoriesQuery>(), CancellationToken)
            .Returns(Result.Failure<IReadOnlyList<IncomeCategoryDto>>(IncomeCategoryErrors.NotFound));

        var result = await controller.GetCategories(CancellationToken);

        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetCategories_Should_ReturnBadRequest_When_HandlerReturnsOtherFailure()
    {
        var controller = CreateController(_sender, "user-1");
        _sender
            .Send(Arg.Any<GetIncomeCategoriesQuery>(), CancellationToken)
            .Returns(Result.Failure<IReadOnlyList<IncomeCategoryDto>>(IncomeCategoryErrors.AccessDenied));

        var result = await controller.GetCategories(CancellationToken);

        result.Should().BeOfType<BadRequestObjectResult>();
    }
}
