using Asp.Versioning;
using Fintrack.Server.Controllers.Expenses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fintrack.Server.Controllers.Expenses;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/expenses")]
public class ExpensesController : ControllerBase
{
    private readonly ISender _sender;

    public ExpensesController(ISender sender)
    {
        _sender = sender;
    }

    // ═══════════════════════════════════════════════════════════════
    // POST: api/v1/expenses
    // ═══════════════════════════════════════════════════════════════
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] RequestCreateExpense request,
        CancellationToken cancellationToken)
    {
        request.UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        var resultId = await _sender.Send(request.ToCommand(), cancellationToken);

        return Created($"api/v1/expenses/{resultId}", resultId);
    }
}
