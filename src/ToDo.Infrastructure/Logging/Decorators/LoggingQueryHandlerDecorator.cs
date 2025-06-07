using Humanizer;
using Microsoft.Extensions.Logging;
using ToDo.Application.Abstractions;

namespace ToDo.Infrastructure.Logging.Decorators;

public class LoggingQueryHandlerDecorator<TQuery, TResult>(
    IQueryHandler<TQuery, TResult> queryHandler,
    ILogger<LoggingQueryHandlerDecorator<TQuery, TResult>> logger) : IQueryHandler<TQuery, TResult>
    where TQuery : class, IQuery<TResult>
{
    public async Task<TResult> HandleAsync(TQuery query)
    {
        var queryName = typeof(TQuery).Name.Underscore();
        logger.LogInformation("Started handling query: {QueryName} with data: {@Query}", queryName, query);
        var result = await queryHandler.HandleAsync(query);
        logger.LogInformation("Completed handling query: {QueryName} with data: {@Query}", queryName, query);
        return result;
    }
}