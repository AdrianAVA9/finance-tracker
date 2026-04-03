using Asp.Versioning;
using Fintrack.Server.Application.Transactions.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Fintrack.Server.Api.Controllers.Transactions
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/transactions")]
    public class TransactionsController : ControllerBase
    {
        private readonly ISender _sender;

        public TransactionsController(ISender sender)
        {
            _sender = sender;
        }

        // ═══════════════════════════════════════════════════════════════
        // GET: api/v1/transactions
        // ═══════════════════════════════════════════════════════════════
        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] int year,
            [FromQuery] int month,
            [FromQuery] string type = "All",
            CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            if (year == 0) year = DateTime.UtcNow.Year;
            if (month == 0) month = DateTime.UtcNow.Month;

            var result = await _sender.Send(new GetTransactionsQuery(userId, year, month, type), cancellationToken);
            return Ok(result);
        }
    }
}
