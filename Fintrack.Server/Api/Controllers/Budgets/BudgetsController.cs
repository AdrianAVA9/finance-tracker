using Asp.Versioning;
using Fintrack.Server.Application.Budgets.Commands.CopyPreviousMonthBudgets;
using Fintrack.Server.Application.Budgets.Commands.DeleteBudget;
using Fintrack.Server.Application.Budgets.Commands.UpsertBudgets;
using Fintrack.Server.Application.Budgets.Queries.GetBudgetDetails;
using Fintrack.Server.Application.Budgets.Queries.GetBudgets;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Fintrack.Server.Api.Controllers.Budgets;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/budgets")]
public class BudgetsController : ApiControllerBase
{
    public BudgetsController(ISender sender) : base(sender)
    {
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int month, [FromQuery] int year)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var query = new GetBudgetsQuery(userId, month, year);
        var result = await Sender.Send(query);
        return HandleResult(result);
    }

    [HttpPost("batch")]
    public async Task<IActionResult> Upsert([FromBody] UpsertBudgetsRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var command = new UpsertBudgetsCommand(userId, request.Month, request.Year, request.Budgets);
        var result = await Sender.Send(command);
        return HandleResult(result);
    }

    [HttpPost("copy-previous")]
    public async Task<IActionResult> CopyPrevious([FromBody] CopyPreviousRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var command = new CopyPreviousMonthBudgetsCommand(userId, request.Month, request.Year);
        var result = await Sender.Send(command);
        return HandleResult(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var command = new DeleteBudgetCommand(id, userId);
        var result = await Sender.Send(command);
        return HandleResult(result);
    }

    [HttpGet("{id:int}/details")]
    public async Task<IActionResult> GetDetails(int id, [FromQuery] int year, [FromQuery] int month)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var query = new GetBudgetDetailsQuery(id, userId, month, year);
        var result = await Sender.Send(query);
        return HandleResult(result);
    }
}

public record UpsertBudgetsRequest(int Month, int Year, List<BudgetEntryDto> Budgets);
public record CopyPreviousRequest(int Month, int Year);
