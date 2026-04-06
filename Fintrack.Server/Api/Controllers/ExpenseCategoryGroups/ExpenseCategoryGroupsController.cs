using Fintrack.Server.Application.ExpenseCategoryGroups.Commands.CreateExpenseCategoryGroup;
using Fintrack.Server.Application.ExpenseCategoryGroups.Commands.DeleteExpenseCategoryGroup;
using Fintrack.Server.Application.ExpenseCategoryGroups.Commands.UpdateExpenseCategoryGroup;
using Fintrack.Server.Application.ExpenseCategoryGroups.Queries.GetAllExpenseCategoryGroups;
using Fintrack.Server.Application.ExpenseCategoryGroups.Queries.GetExpenseCategoryGroupById;
using Fintrack.Server.Domain.ExpenseCategories;
using Fintrack.Server.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fintrack.Server.Api.Controllers.ExpenseCategoryGroups;

[Authorize]
[ApiController]
[Route("api/v1/expensecategorygroups")]
public sealed class ExpenseCategoryGroupsController : ApiControllerBase
{
    public ExpenseCategoryGroupsController(ISender sender)
        : base(sender)
    {
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
        var result = await Sender.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet]
    [HasPermission(Permissions.CategoriesRead)]
    [ProducesResponseType(typeof(IReadOnlyList<ExpenseCategoryGroup>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetAllExpenseCategoryGroupsQuery(GetUserId());
        var result = await Sender.Send(query, cancellationToken);
        return HandleResult(result);
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

        var result = await Sender.Send(command, cancellationToken);
        return HandleResult(result);
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

        var result = await Sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return NoContent();
    }
}
