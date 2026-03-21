using BuildingBlocks.CQRS;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Behaviors;

/// <summary>
/// MediatR 管道行为：命令请求的参数验证中间件<br/>
/// 功能：在执行 <see cref="ICommand{TResponse}"/> 类型的请求处理器前，自动通过 FluentValidation 验证参数合法性<br/>
/// 设计思想：基于 AOP 思想，将参数验证作为横切关注点，避免重复编写验证逻辑
/// </summary>
/// <typeparam name="TRequest">请求类型（必须实现 <see cref="ICommand{TResponse}"/> 接口）</typeparam>
/// <typeparam name="TResponse">命令执行后的响应类型，与 <see cref="ICommand{TResponse}"/> 的泛型参数一致</typeparam>
public class ValidationBehavior<TRequest, TResponse>
    (IEnumerable<IValidator<TRequest>> validators,
     ILogger<ValidationBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
{
    /// <summary>
    /// 管道处理核心方法：执行参数验证，验证通过则调用后续处理器
    /// </summary>
    /// <param name="request">待处理的 <see cref="ICommand{TResponse}"/> 类型请求实例</param>
    /// <param name="next">管道下一个处理委托（最终指向 <see cref="IRequestHandler{TRequest,TResponse}"/> 处理器）</param>
    /// <param name="cancellationToken">异步取消令牌</param>
    /// <returns>命令处理器的响应结果</returns>
    /// <exception cref="ValidationException">验证失败时抛出，包含所有具体的验证错误详情</exception>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            
            var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();
            if (failures.Count != 0)
            {
                logger.LogWarning("Validation errors - {CommandType} - Command: {@Command} - Errors: {@ValidationErrors}",
                                   typeof(TRequest).Name, request, failures);
                throw new ValidationException($"Validation errors for type {typeof(TRequest).Name}", failures);
            }
        }

        return await next(cancellationToken);
    }

}
