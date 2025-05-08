using Humanizer;
using Microsoft.AspNetCore.Http;
using ToDo.Core.Exceptions;

namespace ToDo.Infrastructure.Middlewares;

internal sealed class ExceptionMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, response) = exception switch
        {
            CustomException => (StatusCodes.Status400BadRequest,
                new Response(exception.GetType().Name.Underscore().Replace("_exception", string.Empty),
                    exception.Message)),
            _ => (StatusCodes.Status500InternalServerError, new Response("error", "There was an error."))
        };
        
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(response);
    }

    private record Response(string Code, string Reason);
}