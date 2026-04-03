using Asp.Versioning;
using Fintrack.Server.Application.Budgets.Commands;
using Fintrack.Server.Application.Budgets.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Fintrack.Server.Api.Controllers.Budgets;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/budgets")]
public class BudgetsController : ControllerBase
{
    private readonly ISender _sender;

    public BudgetsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int month, [FromQuery] int year)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var query = new GetBudgetsQuery(userId, month, year);
        var result = await _sender.Send(query);
        return Ok(result);
    }

    [HttpPost("batch")]
    public async Task<IActionResult> Upsert([FromBody] UpsertBudgetsRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var command = new UpsertBudgetsCommand(userId, request.Month, request.Year, request.Budgets);
        await _sender.Send(command);
        return Ok();
    }

    [HttpPost("copy-previous")]
    public async Task<IActionResult> CopyPrevious([FromBody] CopyPreviousRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var command = new CopyPreviousMonthBudgetsCommand(userId, request.Month, request.Year);
        await _sender.Send(command);
        return Ok();
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var command = new DeleteBudgetCommand(id, userId);
        await _sender.Send(command);
        return NoContent();
    }

    [HttpGet("{id}/details")]
    public async Task<IActionResult> GetDetails(int id, [FromQuery] int year, [FromQuery] int month)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var query = new GetBudgetDetailsQuery(id, userId, month, year);
        var result = await _sender.Send(query);

        if (result == null) return NotFound();

        return Ok(result);
    }
}

public record UpsertBudgetsRequest(int Month, int Year, List<BudgetEntryDto> Budgets);
public record CopyPreviousRequest(int Month, int Year);
