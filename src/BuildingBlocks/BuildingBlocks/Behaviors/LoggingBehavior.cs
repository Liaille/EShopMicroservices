using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;

namespace BuildingBlocks.Behaviors;

/// <summary>
/// MediatR 管道行为：为所有请求提供统一的日志记录和性能监控
/// <para>实现 <see cref="IPipelineBehavior{TRequest, TResponse}"/> 接口，接入 MediatR 请求处理管道</para>
/// </summary>
/// <typeparam name="TRequest">请求类型（需实现 <see cref="IRequest{TResponse}"/>，且不能为空）</typeparam>
/// <typeparam name="TResponse">响应类型（不能为空）</typeparam>
/// <remarks>
/// 核心能力：
/// <list type="number">
/// <item>记录请求开始/结束的详细日志（包含请求/响应内容）；</item>
/// <item>监控请求处理耗时，对超过3秒的请求输出 <see cref="LogLevel.Warning"/> 级别的性能警告；</item>
/// <item>遵循 AOP 思想，无需在每个 <see cref="IRequestHandler{TRequest, TResponse}"/> 中重复编写日志逻辑。</item>
/// </list>
/// </remarks>
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
