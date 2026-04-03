using Asp.Versioning;
using Fintrack.Server.Api.Controllers.Expenses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Fintrack.Server.Application.Expenses.Queries;
using Fintrack.Server.Application.Expenses.Commands;
using Fintrack.Server.Domain.Enums;

namespace Fintrack.Server.Api.Controllers.Expenses;

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
    // GET: api/v1/expenses/{id}
    // ═══════════════════════════════════════════════════════════════
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _sender.Send(new GetExpenseByIdQuery(id, userId), cancellationToken);
        return result != null ? Ok(result) : NotFound();
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

    // ═══════════════════════════════════════════════════════════════
    // PUT: api/v1/expenses/{id}
    // ═══════════════════════════════════════════════════════════════
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateExpenseRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var command = new UpdateExpenseCommand(
            id,
            userId,
            request.TotalAmount,
            request.Date,
            request.Merchant,
            request.InvoiceNumber,
            request.InvoiceImageUrl,
            request.IsRecurring,
            request.Frequency,
            request.Items.Select(i => new UpdateExpenseItemDto(i.Id, i.CategoryId, i.ItemAmount, i.Description)).ToList()
        );

        var success = await _sender.Send(command, cancellationToken);
        return success ? NoContent() : NotFound();
    }

    // ═══════════════════════════════════════════════════════════════
    // DELETE: api/v1/expenses/{id}
    // ═══════════════════════════════════════════════════════════════
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var success = await _sender.Send(new DeleteExpenseCommand(id, userId), cancellationToken);
        return success ? NoContent() : NotFound();
    }
}

public record UpdateExpenseRequest(
    decimal TotalAmount,
    DateTime Date,
    string? Merchant,
    string? InvoiceNumber,
    string? InvoiceImageUrl,
    bool IsRecurring,
    RecurringFrequency? Frequency,
    List<UpdateExpenseItemRequestDto> Items
);

public record UpdateExpenseItemRequestDto(int? Id, int CategoryId, decimal ItemAmount, string? Description);
