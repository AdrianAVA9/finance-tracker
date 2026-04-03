using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Fintrack.Server.Application.ExpenseCategories.Commands;
using Fintrack.Server.Application.ExpenseCategories.Queries;
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

namespace Fintrack.Server.Api.Controllers.ExpenseCategories
{
    [Authorize]
    [ApiController]
    [Route("api/v1/expensecategories")]
    public class ExpenseCategoriesController : ControllerBase
    {
        private readonly ISender _sender;

        public ExpenseCategoriesController(ISender sender)
        {
            _sender = sender;
        }

        private string GetUserId() => User.GetUserId() ?? throw new UnauthorizedAccessException();

        [HttpGet("{id:int}")]
        [HasPermission(Permissions.CategoriesRead)]
        [ProducesResponseType(typeof(ExpenseCategory), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(
            int id,
            CancellationToken cancellationToken)
        {
            var query = new GetExpenseCategoryByIdQuery(id, GetUserId());
            var result = await _sender.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet]
        [HasPermission(Permissions.CategoriesRead)]
        [ProducesResponseType(typeof(IReadOnlyList<ExpenseCategory>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var query = new GetAllExpenseCategoriesQuery(GetUserId());
            var result = await _sender.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [HasPermission(Permissions.CategoriesWrite)]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(
            [FromBody] RequestCreateExpenseCategory request,
            CancellationToken cancellationToken)
        {
            var command = new CreateExpenseCategoryCommand(
                request.Name,
                request.Description,
                request.Icon,
                request.Color,
                request.GroupId,
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
            [FromBody] RequestUpdateExpenseCategory request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateExpenseCategoryCommand(
                id,
                request.Name,
                request.Description,
                request.Icon,
                request.Color,
                request.GroupId,
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
            var command = new DeleteExpenseCategoryCommand(id, GetUserId());
            
            await _sender.Send(command, cancellationToken);
            
            return NoContent();
        }
    }
}
