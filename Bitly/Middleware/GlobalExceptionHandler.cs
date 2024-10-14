using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Bitly.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        switch (exception)
        {
            case ValidationException validationException:
                await HandleValidationExceptionAsync(httpContext, validationException);
                return true;
            default:
                return false;
        }
    }

    private async Task HandleValidationExceptionAsync(HttpContext httpContext, ValidationException exception)
    {
        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        httpContext.Response.ContentType = "application/json";
        var problemDetails = new ProblemDetails
        {
            Title = "Validation Error",
            Detail = "One or more validation errors occurred.",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Status = StatusCodes.Status400BadRequest,
            Instance = httpContext.Request.Path
        };

        foreach (var error in exception.Errors)
            problemDetails.Extensions.Add(error.PropertyName, new[] { error.ErrorMessage });

        await httpContext.Response.WriteAsJsonAsync(problemDetails);
    }
}
