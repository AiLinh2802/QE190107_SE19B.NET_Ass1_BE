using Microsoft.AspNetCore.Mvc;
using TaskTrack.Service.Exceptions;

namespace TaskTrack.API.Middleware;

public class ApiExceptionMiddleware(RequestDelegate next, ILogger<ApiExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ServiceException exception)
        {
            context.Response.StatusCode = exception.StatusCode;
            if (exception.Errors is not null)
            {
                await context.Response.WriteAsJsonAsync(new ValidationProblemDetails(exception.Errors)
                {
                    Status = exception.StatusCode,
                    Title = "One or more validation errors occurred."
                });
                return;
            }
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = exception.StatusCode,
                Title = exception.StatusCode == 404 ? "Resource not found" : "Request failed",
                Detail = exception.Message
            });
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled exception while processing {Method} {Path}", context.Request.Method, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An unexpected server error occurred.",
                Detail = "Please try again later."
            });
        }
    }
}
