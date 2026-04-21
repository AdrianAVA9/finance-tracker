using System.Security.Claims;
using Fintrack.Server.Api.Controllers.Expenses;
using Fintrack.Server.Application.Expenses.Commands.CreateExpense;
using Fintrack.Server.Application.Expenses.Commands.DeleteExpense;
using Fintrack.Server.Application.Expenses.Commands.UpdateExpense;
using Fintrack.Server.Application.Expenses.Queries.GetExpenseById;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Expenses;
using Fintrack.Tests.Abstractions;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace Fintrack.Tests.Api.Controllers.Expenses;

/// <summary>
/// Controller-focused unit tests: MediatR is mocked; no HTTP pipeline, EF, or authorization filters.
/// Permission and routing coverage stays in <c>Fintrack.IntegrationTests</c>.
/// </summary>
public sealed class ExpensesControllerTests : BaseUnitTest
{
    private readonly ISender _sender = Mock<ISender>();

    private static ExpensesController CreateController(ISender sender, string? userId = null)
    {
        var controller = new ExpensesController(sender);
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

    // ═══════════════════════════════════════════════════════════════
    // GetById
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public async Task GetById_Should_ReturnUnauthorized_When_UserIdMissing()
    {
        var controller = CreateController(_sender, userId: null);

        var result = await controller.GetById(Guid.NewGuid(), CancellationToken);

        result.Should().BeOfType<UnauthorizedResult>();
        await _sender.DidNotReceive().Send(
            Arg.Any<GetExpenseByIdQuery>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetById_Should_ReturnOk_When_Success()
    {
        var userId = "user-1";
        var expenseId = Guid.NewGuid();
        var controller = CreateController(_sender, userId);
        var dto = new ExpenseDetailsDto(
            expenseId,
            Merchant: "Store",
            TotalAmount: 50m,
            Date: DateTime.UtcNow,
            StatusName: "Completed",
            InvoiceNumber: null,
            InvoiceImageUrl: null,
            Items: [],
            Invoice: null);

        _sender
            .Send(Arg.Any<GetExpenseByIdQuery>(), CancellationToken)
            .Returns(Result.Success(dto));

        var result = await controller.GetById(expenseId, CancellationToken);

        await _sender.Received(1).Send(
            Arg.Is<GetExpenseByIdQuery>(q => q.Id == expenseId && q.UserId == userId),
            CancellationToken);
        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeSameAs(dto);
    }

    [Fact]
    public async Task GetById_Should_ReturnNotFound_When_ExpenseMissing()
    {
        var userId = "user-1";
        var controller = CreateController(_sender, userId);
        _sender
            .Send(Arg.Any<GetExpenseByIdQuery>(), CancellationToken)
            .Returns(Result.Failure<ExpenseDetailsDto>(ExpenseErrors.NotFound));

        var result = await controller.GetById(Guid.NewGuid(), CancellationToken);

        result.Should().BeOfType<NotFoundObjectResult>();
    }

    // ═══════════════════════════════════════════════════════════════
    // Create
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public async Task Create_Should_ReturnUnauthorized_When_UserIdMissing()
    {
        var controller = CreateController(_sender, userId: null);
        var request = new CreateExpenseRequest(
            "Store", 100m, DateTime.UtcNow, null, null,
            [new RequestExpenseItemData(Guid.NewGuid(), 100m, null)]);

        var result = await controller.Create(request, CancellationToken);

        result.Should().BeOfType<UnauthorizedResult>();
        await _sender.DidNotReceive().Send(
            Arg.Any<CreateExpenseCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_Should_ReturnCreated_When_Success()
    {
        var userId = "user-1";
        var controller = CreateController(_sender, userId);
        var categoryId = Guid.NewGuid();
        var request = new CreateExpenseRequest(
            "Store", 80m, new DateTime(2025, 6, 1), "INV-001", null,
            [new RequestExpenseItemData(categoryId, 80m, "Groceries")]);

        var newId = Guid.NewGuid();
        _sender
            .Send(Arg.Any<CreateExpenseCommand>(), CancellationToken)
            .Returns(Result.Success(newId));

        var result = await controller.Create(request, CancellationToken);

        await _sender.Received(1).Send(
            Arg.Is<CreateExpenseCommand>(c =>
                c.UserId == userId &&
                c.TotalAmount == 80m &&
                c.Merchant == "Store" &&
                c.Items.Count == 1 &&
                c.Items[0].CategoryId == categoryId),
            CancellationToken);
        var created = result.Should().BeOfType<CreatedResult>().Subject;
        created.Value.Should().Be(newId);
    }

    [Fact]
    public async Task Create_Should_ReturnBadRequest_When_Failure()
    {
        var userId = "user-1";
        var controller = CreateController(_sender, userId);
        var request = new CreateExpenseRequest(
            "Store", 0m, DateTime.UtcNow, null, null,
            [new RequestExpenseItemData(Guid.NewGuid(), 0m, null)]);

        _sender
            .Send(Arg.Any<CreateExpenseCommand>(), CancellationToken)
            .Returns(Result.Failure<Guid>(ExpenseErrors.InvalidTotalAmount));

        var result = await controller.Create(request, CancellationToken);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    // ═══════════════════════════════════════════════════════════════
    // Update
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public async Task Update_Should_ReturnUnauthorized_When_UserIdMissing()
    {
        var controller = CreateController(_sender, userId: null);
        var request = new UpdateExpenseRequest(
            50m, DateTime.UtcNow, "Store", null, null,
            [new RequestExpenseItemData(Guid.NewGuid(), 50m, null)]);

        var result = await controller.Update(Guid.NewGuid(), request, CancellationToken);

        result.Should().BeOfType<UnauthorizedResult>();
        await _sender.DidNotReceive().Send(
            Arg.Any<UpdateExpenseCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Update_Should_ReturnOk_When_Success()
    {
        var userId = "user-1";
        var expenseId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var controller = CreateController(_sender, userId);
        var request = new UpdateExpenseRequest(
            120m, new DateTime(2025, 7, 1), "Updated Store", null, null,
            [new RequestExpenseItemData(categoryId, 120m, "Updated")]);

        _sender
            .Send(Arg.Any<UpdateExpenseCommand>(), CancellationToken)
            .Returns(Result.Success());

        var result = await controller.Update(expenseId, request, CancellationToken);

        await _sender.Received(1).Send(
            Arg.Is<UpdateExpenseCommand>(c =>
                c.Id == expenseId &&
                c.UserId == userId &&
                c.TotalAmount == 120m &&
                c.Merchant == "Updated Store"),
            CancellationToken);
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task Update_Should_ReturnNotFound_When_ExpenseMissing()
    {
        var userId = "user-1";
        var controller = CreateController(_sender, userId);
        var request = new UpdateExpenseRequest(
            50m, DateTime.UtcNow, null, null, null,
            [new RequestExpenseItemData(Guid.NewGuid(), 50m, null)]);

        _sender
            .Send(Arg.Any<UpdateExpenseCommand>(), CancellationToken)
            .Returns(Result.Failure(ExpenseErrors.NotFound));

        var result = await controller.Update(Guid.NewGuid(), request, CancellationToken);

        result.Should().BeOfType<NotFoundObjectResult>();
    }

    // ═══════════════════════════════════════════════════════════════
    // Delete
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public async Task Delete_Should_ReturnUnauthorized_When_UserIdMissing()
    {
        var controller = CreateController(_sender, userId: null);

        var result = await controller.Delete(Guid.NewGuid(), CancellationToken);

        result.Should().BeOfType<UnauthorizedResult>();
        await _sender.DidNotReceive().Send(
            Arg.Any<DeleteExpenseCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Delete_Should_ReturnOk_When_Success()
    {
        var userId = "user-1";
        var expenseId = Guid.NewGuid();
        var controller = CreateController(_sender, userId);

        _sender
            .Send(Arg.Any<DeleteExpenseCommand>(), CancellationToken)
            .Returns(Result.Success());

        var result = await controller.Delete(expenseId, CancellationToken);

        await _sender.Received(1).Send(
            Arg.Is<DeleteExpenseCommand>(c => c.Id == expenseId && c.UserId == userId),
            CancellationToken);
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task Delete_Should_ReturnNotFound_When_ExpenseMissing()
    {
        var userId = "user-1";
        var controller = CreateController(_sender, userId);

        _sender
            .Send(Arg.Any<DeleteExpenseCommand>(), CancellationToken)
            .Returns(Result.Failure(ExpenseErrors.NotFound));

        var result = await controller.Delete(Guid.NewGuid(), CancellationToken);

        result.Should().BeOfType<NotFoundObjectResult>();
    }
}
