using Asp.Versioning;
using Fintrack.Server.Application.Expenses.Commands.CreateExpense;
using Fintrack.Server.Application.Expenses.Commands.DeleteExpense;
using Fintrack.Server.Application.Expenses.Commands.UpdateExpense;
using Fintrack.Server.Application.Expenses.Queries.GetExpenseById;
using Fintrack.Server.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Fintrack.Server.Api.Controllers.Expenses;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/expenses")]
public class ExpensesController : ApiControllerBase
{
    public ExpensesController(ISender sender) : base(sender)
    {
    }

    [HttpGet("{id:guid}")]
    [HasPermission(Permissions.ExpensesRead)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var query = new GetExpenseByIdQuery(id, userId);
        var result = await Sender.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost]
    [HasPermission(Permissions.ExpensesWrite)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateExpenseRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var command = new CreateExpenseCommand(
            userId,
            request.TotalAmount,
            request.Date,
            request.Merchant,
            request.InvoiceNumber,
            request.InvoiceImageUrl,
            request.Items.Select(i => new ExpenseItemDto(i.CategoryId, i.ItemAmount, i.Description)).ToList());

        var result = await Sender.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return Created($"api/v1/expenses/{result.Value}", result.Value);
        }

        return HandleFailure(result);
    }

    [HttpPut("{id:guid}")]
    [HasPermission(Permissions.ExpensesWrite)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateExpenseRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var command = new UpdateExpenseCommand(
            id,
            userId,
            request.TotalAmount,
            request.Date,
            request.Merchant,
            request.InvoiceNumber,
            request.InvoiceImageUrl,
            request.Items.Select(i => new UpdateExpenseItemDto(i.CategoryId, i.ItemAmount, i.Description)).ToList());

        var result = await Sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    [HasPermission(Permissions.ExpensesDelete)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var command = new DeleteExpenseCommand(id, userId);
        var result = await Sender.Send(command, cancellationToken);
        return HandleResult(result);
    }
}
