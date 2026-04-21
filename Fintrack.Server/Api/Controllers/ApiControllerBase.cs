using Fintrack.Server.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fintrack.Server.Api.Controllers;

public abstract class ApiControllerBase : ControllerBase
{
    protected readonly ISender Sender;

    protected ApiControllerBase(ISender sender)
    {
        Sender = sender;
    }

    protected IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
        {
            return Ok();
        }

        return HandleFailure(result);
    }

    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return HandleFailure(result);
    }

    protected IActionResult HandleFailure(Result result)
    {
        return result.Error switch
        {
            { Code: "Error.NullValue" } => BadRequest(CreateProblemDetails("Bad Request", result.Error)),
            _ when result.Error.Code.Contains("NotFound") => NotFound(CreateProblemDetails("Not Found", result.Error)),
            _ => BadRequest(CreateProblemDetails("Bad Request", result.Error))
        };
    }

    private static ProblemDetails CreateProblemDetails(
        string title,
        Error error,
        string? detail = null,
        IEnumerable<object>? errors = null) =>
        new()
        {
            Title = title,
            Type = error.Code,
            Detail = error.Description,
            Status = title == "Not Found" ? StatusCodes.Status404NotFound : StatusCodes.Status400BadRequest,
            Extensions = { { nameof(errors), errors ?? new[] { error } } }
        };
}
