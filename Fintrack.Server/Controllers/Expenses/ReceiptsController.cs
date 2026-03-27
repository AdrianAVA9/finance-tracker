using System.Security.Claims;
using Fintrack.Server.Application.Expenses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace Fintrack.Server.Controllers.Expenses;

[Authorize]
[ApiController]
[Route("api/v1/receipts")]
public class ReceiptsController : ControllerBase
{
    private readonly ReceiptProcessingService _receiptProcessor;

    public ReceiptsController(ReceiptProcessingService receiptProcessor)
    {
        _receiptProcessor = receiptProcessor;
    }

    [HttpPost("process")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ProcessReceipt(IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("A valid image file is required.");
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        using var stream = file.OpenReadStream();
        var mimeType = file.ContentType;

        try
        {
            var expense = await _receiptProcessor.ProcessReceiptAsync(stream, mimeType, userId, cancellationToken);
            
            // To avoid circular references in JSON serialization from Entity framework navigation properties, 
            // you'd typically return a DTO here. Returning the raw entity for demonstration based on the setup.
            return Ok(expense);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred during OCR processing: {ex.Message}");
        }
    }
}
