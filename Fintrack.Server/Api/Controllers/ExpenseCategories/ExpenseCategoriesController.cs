using Fintrack.Server.Application.ExpenseCategories.Commands.CreateExpenseCategory;
using Fintrack.Server.Application.ExpenseCategories.Commands.DeleteExpenseCategory;
using Fintrack.Server.Application.ExpenseCategories.Commands.UpdateExpenseCategory;
using Fintrack.Server.Application.ExpenseCategories.Queries.GetAllExpenseCategories;
using Fintrack.Server.Application.ExpenseCategories.Queries.GetExpenseCategoryById;
using Fintrack.Server.Application.ExpenseCategories.Queries.GetUserOwnedExpenseCategories;
using Fintrack.Server.Domain.ExpenseCategories;
using Fintrack.Server.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fintrack.Server.Api.Controllers.ExpenseCategories;

[Authorize]
[ApiController]
[Route("api/v1/expensecategories")]
public sealed class ExpenseCategoriesController : ApiControllerBase
{
    public ExpenseCategoriesController(ISender sender)
        : base(sender)
    {
    }

    private string GetUserId() => User.GetUserId() ?? throw new UnauthorizedAccessException();

    [HttpGet("owned")]
    [HasPermission(Permissions.CategoriesRead)]
    [ProducesResponseType(typeof(IReadOnlyList<ExpenseCategory>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserOwned(CancellationToken cancellationToken)
    {
        var query = new GetUserOwnedExpenseCategoriesQuery(GetUserId());
        var result = await Sender.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{id:guid}")]
    [HasPermission(Permissions.CategoriesRead)]
    [ProducesResponseType(typeof(ExpenseCategory), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetExpenseCategoryByIdQuery(id, GetUserId());
        var result = await Sender.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet]
    [HasPermission(Permissions.CategoriesRead)]
    [ProducesResponseType(typeof(IReadOnlyList<ExpenseCategory>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetAllExpenseCategoriesQuery(GetUserId());
        var result = await Sender.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost]
    [HasPermission(Permissions.CategoriesWrite)]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
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

        var result = await Sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value },
            result.Value);
    }

    [HttpPut("{id:guid}")]
    [HasPermission(Permissions.CategoriesWrite)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
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

        var result = await Sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    [HasPermission(Permissions.CategoriesDelete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteExpenseCategoryCommand(id, GetUserId());

        var result = await Sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return NoContent();
    }
}
