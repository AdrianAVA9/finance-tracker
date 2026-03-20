using Fintrack.Server.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Fintrack.Server.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

                var problemDetails = CreateProblemDetails(exception, _env.IsDevelopment());

                context.Response.StatusCode = problemDetails.Status ?? 500;
                await context.Response.WriteAsJsonAsync(problemDetails);
            }
        }

        private static ProblemDetails CreateProblemDetails(Exception exception, bool isDevelopment)
        {
            if (isDevelopment)
            {
                // Expose the real exception in Development for easier debugging
                return new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = exception.GetType().Name,
                    Detail = $"{exception.Message}\n\nInner Exception: {exception.InnerException?.Message}\n\nStack Trace: {exception.StackTrace}"
                };
            }

            return exception switch
            {
                ValidationException validationException => new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Validation Error",
                    Detail = "One or more validation errors occurred.",
                    Extensions = { ["errors"] = validationException.Errors }
                },
                NotFoundException => new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Not Found",
                    Detail = exception.Message
                },
                UnauthorizedAccessException => new ProblemDetails
                {
                    Status = StatusCodes.Status403Forbidden,
                    Title = "Forbidden",
                    Detail = exception.Message
                },
                DomainException => new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Bad Request",
                    Detail = exception.Message
                },
                _ => new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Server Error",
                    Detail = "An unexpected error occurred."
                }
            };
        }
    }

    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(
            this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
