using Asp.Versioning;
using Fintrack.Server.Api.Controllers;
using Fintrack.Server.Application.IncomeCategories.Commands.CreateIncomeCategory;
using Fintrack.Server.Application.IncomeCategories.Commands.UpdateIncomeCategory;
using Fintrack.Server.Application.IncomeCategories.Queries.GetIncomeCategories;
using IncomeCategoryDto = Fintrack.Server.Application.IncomeCategories.Queries.GetIncomeCategories.IncomeCategoryDto;
using Fintrack.Server.Application.IncomeCategories.Queries.GetIncomeCategoryById;
using Fintrack.Server.Application.IncomeCategories.Queries.GetUserOwnedIncomeCategories;
using Fintrack.Server.Application.Incomes.Commands.CreateIncome;
using Fintrack.Server.Application.Incomes.Commands.DeleteIncome;
using Fintrack.Server.Application.Incomes.Commands.UpdateIncome;
using Fintrack.Server.Application.Incomes.Queries.GetIncomeById;
using Fintrack.Server.Domain.Enums;
using Fintrack.Server.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Fintrack.Server.Api.Controllers.Incomes;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/incomes")]
public class IncomesController : ApiControllerBase
{
    public IncomesController(ISender sender)
        : base(sender)
    {
    }

    [HttpGet("categories")]
    [HasPermission(Permissions.IncomesRead)]
    public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await Sender.Send(new GetIncomeCategoriesQuery(userId), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("categories/owned")]
    [HasPermission(Permissions.IncomesRead)]
    public async Task<IActionResult> GetUserOwnedCategories(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await Sender.Send(new GetUserOwnedIncomeCategoriesQuery(userId), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("categories/{id:guid}")]
    [HasPermission(Permissions.IncomesRead)]
    [ProducesResponseType(typeof(IncomeCategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCategoryById(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await Sender.Send(new GetIncomeCategoryByIdQuery(id, userId), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("categories")]
    [HasPermission(Permissions.IncomesWrite)]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCategory(
        [FromBody] RequestCreateIncomeCategory request,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var command = new CreateIncomeCategoryCommand(
            request.Name,
            request.Icon,
            request.Color,
            userId);

        var result = await Sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return Created($"api/v1/incomes/categories/{result.Value}", result.Value);
    }

    [HttpPut("categories/{id:guid}")]
    [HasPermission(Permissions.IncomesWrite)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCategory(
        Guid id,
        [FromBody] RequestUpdateIncomeCategory request,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var command = new UpdateIncomeCategoryCommand(
            id,
            request.Name,
            request.Icon,
            request.Color,
            userId);

        var result = await Sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{id:guid}")]
    [HasPermission(Permissions.IncomesRead)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await Sender.Send(new GetIncomeByIdQuery(id, userId), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost]
    [HasPermission(Permissions.IncomesWrite)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateIncomeRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var command = new CreateIncomeCommand(
            userId,
            request.Source,
            request.Amount,
            request.CategoryId,
            request.Date,
            request.Notes,
            request.IsRecurring,
            request.Frequency,
            request.NextDate);

        var result = await Sender.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return Created($"api/v1/incomes/{result.Value}", result.Value);
        }

        return HandleFailure(result);
    }

    [HttpPut("{id:guid}")]
    [HasPermission(Permissions.IncomesWrite)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateIncomeRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var command = new UpdateIncomeCommand(
            id,
            userId,
            request.Source,
            request.Amount,
            request.CategoryId,
            request.Date,
            request.Notes,
            request.IsRecurring,
            request.Frequency,
            request.NextDate);

        var result = await Sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    [HasPermission(Permissions.IncomesDelete)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await Sender.Send(new DeleteIncomeCommand(id, userId), cancellationToken);
        return HandleResult(result);
    }
}

public record RequestCreateIncomeCategory(
    string Name,
    string? Icon,
    string? Color);

public record RequestUpdateIncomeCategory(
    string Name,
    string? Icon,
    string? Color);

public record CreateIncomeRequest(
    string Source,
    decimal Amount,
    Guid CategoryId,
    DateTime Date,
    string? Notes,
    bool IsRecurring,
    RecurringFrequency? Frequency,
    DateTime? NextDate);

public record UpdateIncomeRequest(
    string Source,
    decimal Amount,
    Guid CategoryId,
    DateTime Date,
    string? Notes,
    bool IsRecurring,
    RecurringFrequency? Frequency,
    DateTime? NextDate);
