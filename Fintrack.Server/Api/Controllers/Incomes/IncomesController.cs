using Asp.Versioning;
using Fintrack.Server.Api.Controllers;
using Fintrack.Server.Application.IncomeCategories.Queries.GetIncomeCategories;
using Fintrack.Server.Application.Incomes.Commands;
using Fintrack.Server.Application.Incomes.Queries;
using Fintrack.Server.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Fintrack.Server.Api.Controllers.Incomes
{
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
        public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await Sender.Send(new GetIncomeCategoriesQuery(userId), cancellationToken);
            return HandleResult(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await Sender.Send(new GetIncomeByIdQuery(id, userId));
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

            var id = await Sender.Send(command);
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

            var success = await Sender.Send(command);
            return success ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var success = await Sender.Send(new DeleteIncomeCommand(id, userId));
            return success ? NoContent() : NotFound();
        }
    }

    public record CreateIncomeRequest(
        string Source,
        decimal Amount,
        Guid CategoryId,
        DateTime Date,
        string? Notes,
        bool IsRecurring,
        RecurringFrequency? Frequency,
        DateTime? NextDate
    );

    public record UpdateIncomeRequest(
        string Source,
        decimal Amount,
        Guid CategoryId,
        DateTime Date,
        string? Notes,
        bool IsRecurring,
        RecurringFrequency? Frequency,
        DateTime? NextDate
    );
}
