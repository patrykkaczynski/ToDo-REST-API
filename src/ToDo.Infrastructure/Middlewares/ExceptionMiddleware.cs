using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ToDo.Application.Exceptions;
using ToDo.Core.Exceptions;
using ToDo.Infrastructure.Exceptions;
using ToDo.Shared.Errors;

namespace ToDo.Infrastructure.Middlewares;

internal sealed class ExceptionMiddleware(ILogger<ExceptionMiddleware> logger): IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, exception.Message);
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, response) = exception switch
        {
            ToDoTaskNotFoundException or NoIncomingFilterPolicyFoundException => (StatusCodes.Status404NotFound,
                CreateError(exception)),
            CustomException => (StatusCodes.Status400BadRequest, CreateError(exception)),
            _ => (StatusCodes.Status500InternalServerError, new Error("error", "There was an error."))
        };

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(response);
    }
    
    private static Error CreateError(Exception ex) =>
        new Error(ex.GetType().Name.Underscore().Replace("_exception", string.Empty), ex.Message);
}