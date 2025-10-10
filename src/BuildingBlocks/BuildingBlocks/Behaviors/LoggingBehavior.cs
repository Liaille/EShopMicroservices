using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;

namespace BuildingBlocks.Behaviors;

public class LoggingBehavior<TRequest, TResponse>
    (ILogger<LoggingBehavior<TRequest, TResponse>> logger) 
    : IPipelineBehavior<TRequest, TResponse> 
    where TRequest : notnull, IRequest<TResponse> 
    where TResponse : notnull
{
    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        string requestType = typeof(TRequest).Name;
        logger.LogInformation("[START] Handle {RequestType} with content: {Request}", requestType, JsonSerializer.Serialize(request));

        Stopwatch sw = Stopwatch.StartNew();
        var response = next(cancellationToken);
        sw.Stop();

        if (sw.ElapsedMilliseconds > 3000) logger.LogWarning("[PERFORMANCE] The request {RequestType} took {TimeTaken}ms", requestType, sw.ElapsedMilliseconds);
        logger.LogInformation("[END] Handle {RequestType} with response: {Response}", requestType, JsonSerializer.Serialize(response));

        return response;
    }
}
