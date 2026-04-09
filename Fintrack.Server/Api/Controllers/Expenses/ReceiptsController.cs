using System.Security.Claims;
using Asp.Versioning;
using Fintrack.Server.Api.Controllers;
using Fintrack.Server.Application.Expenses;
using Fintrack.Server.Application.Expenses.Commands.EnqueuePendingReceipt;
using Fintrack.Server.Application.Expenses.Queries.GetPendingReceiptJob;
using Fintrack.Server.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fintrack.Server.Api.Controllers.Expenses;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/receipts")]
public sealed class ReceiptsController : ApiControllerBase
{
    private readonly ReceiptProcessingService _receiptProcessor;

    public ReceiptsController(ISender sender, ReceiptProcessingService receiptProcessor)
        : base(sender)
    {
        _receiptProcessor = receiptProcessor;
    }

    [HttpPost("process")]
    [HasPermission(Permissions.ExpensesWrite)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ProcessReceipt(IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest("A valid image file is required.");
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        using var stream = file.OpenReadStream();
        var mimeType = file.ContentType ?? "application/octet-stream";

        try
        {
            var expense = await _receiptProcessor.ProcessReceiptAsync(stream, mimeType, userId, cancellationToken);
            return Ok(expense);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred during OCR processing: {ex.Message}");
        }
    }

    /// <summary>
    /// Upload a receipt for asynchronous AI extraction. Poll <see cref="GetReceiptJob"/> until status is Completed or Failed.
    /// </summary>
    [HttpPost("queue")]
    [HasPermission(Permissions.ExpensesWrite)]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> QueueReceipt(IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest("A valid file is required.");
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        await using var ms = new MemoryStream();
        await file.CopyToAsync(ms, cancellationToken);

        var command = new EnqueuePendingReceiptCommand(
            userId,
            ms.ToArray(),
            file.ContentType ?? "application/octet-stream",
            file.FileName);

        var result = await Sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return Accepted(
            $"/api/v1/receipts/jobs/{result.Value}",
            new { id = result.Value, status = "Queued" });
    }

    [HttpGet("jobs/{id:guid}", Name = nameof(GetReceiptJob))]
    [HasPermission(Permissions.ExpensesRead)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetReceiptJob(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var result = await Sender.Send(new GetPendingReceiptJobQuery(id, userId), cancellationToken);
        return HandleResult(result);
    }
}
