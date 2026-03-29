using Asp.Versioning;
using Fintrack.Server.Application.Incomes.Commands;
using Fintrack.Server.Application.Incomes.Queries;
using Fintrack.Server.Models.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Fintrack.Server.Controllers.Incomes
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/incomes")]
    public class IncomesController : ControllerBase
    {
        private readonly ISender _sender;

        public IncomesController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _sender.Send(new GetIncomeCategoriesQuery(userId));
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _sender.Send(new GetIncomeByIdQuery(id, userId));
            return result != null ? Ok(result) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateIncomeRequest request)
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
                request.NextDate
            );

            var id = await _sender.Send(command);
            return Ok(new { id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateIncomeRequest request)
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
                request.NextDate
            );

            var success = await _sender.Send(command);
            return success ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var success = await _sender.Send(new DeleteIncomeCommand(id, userId));
            return success ? NoContent() : NotFound();
        }
    }

    public record CreateIncomeRequest(
        string Source,
        decimal Amount,
        int CategoryId,
        DateTime Date,
        string? Notes,
        bool IsRecurring,
        RecurringFrequency? Frequency,
        DateTime? NextDate
    );

    public record UpdateIncomeRequest(
        string Source,
        decimal Amount,
        int CategoryId,
        DateTime Date,
        string? Notes,
        bool IsRecurring,
        RecurringFrequency? Frequency,
        DateTime? NextDate
    );
}
