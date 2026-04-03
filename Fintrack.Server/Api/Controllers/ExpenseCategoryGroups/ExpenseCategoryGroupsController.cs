using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Fintrack.Server.Application.ExpenseCategoryGroups.Commands;
using Fintrack.Server.Application.ExpenseCategoryGroups.Queries;
using Fintrack.Server.Infrastructure.Authorization;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Budgets;
using Fintrack.Server.Domain.Enums;
using Fintrack.Server.Domain.Exceptions;
using Fintrack.Server.Domain.ExpenseCategories;
using Fintrack.Server.Domain.Expenses;
using Fintrack.Server.Domain.Incomes;
using Fintrack.Server.Domain.Invoices;
using Fintrack.Server.Domain.SavingsGoals;
using Fintrack.Server.Domain.Users;

namespace Fintrack.Server.Api.Controllers.ExpenseCategoryGroups
{
    [Authorize]
    [ApiController]
    [Route("api/v1/expensecategorygroups")]
    public class ExpenseCategoryGroupsController : ControllerBase
    {
        private readonly ISender _sender;

        public ExpenseCategoryGroupsController(ISender sender)
        {
            _sender = sender;
        }

        private string GetUserId() => User.GetUserId() ?? throw new UnauthorizedAccessException();

        [HttpGet("{id:int}")]
        [HasPermission(Permissions.CategoriesRead)]
        [ProducesResponseType(typeof(ExpenseCategoryGroup), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(
            int id,
            CancellationToken cancellationToken)
        {
            var query = new GetExpenseCategoryGroupByIdQuery(id, GetUserId());
            var result = await _sender.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet]
        [HasPermission(Permissions.CategoriesRead)]
        [ProducesResponseType(typeof(IReadOnlyList<ExpenseCategoryGroup>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var query = new GetAllExpenseCategoryGroupsQuery(GetUserId());
            var result = await _sender.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [HasPermission(Permissions.CategoriesWrite)]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(
            [FromBody] RequestCreateExpenseCategoryGroup request,
            CancellationToken cancellationToken)
        {
            var command = new CreateExpenseCategoryGroupCommand(
                request.Name,
                request.Description,
                GetUserId());

            var resultId = await _sender.Send(command, cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = resultId },
                resultId);
        }

        [HttpPut("{id:int}")]
        [HasPermission(Permissions.CategoriesWrite)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] RequestUpdateExpenseCategoryGroup request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateExpenseCategoryGroupCommand(
                id,
                request.Name,
                request.Description,
                GetUserId());

            await _sender.Send(command, cancellationToken);
            return Ok();
        }

        [HttpDelete("{id:int}")]
        [HasPermission(Permissions.CategoriesDelete)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(
            int id,
            CancellationToken cancellationToken)
        {
            var command = new DeleteExpenseCategoryGroupCommand(id, GetUserId());
            
            await _sender.Send(command, cancellationToken);
            
            return NoContent();
        }
    }
}
