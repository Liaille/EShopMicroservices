using MediatR;

namespace BuildingBlocks.CQRS;

/// <summary>
/// 基于MediatR的通用命令处理器接口，处理类型为TCommand的命令请求，没有返回值
/// </summary>
/// <typeparam name="TCommand">命令请求</typeparam>
public interface ICommandHandler<in TCommand> 
    : IRequestHandler<TCommand, Unit> 
    where TCommand : ICommand
{
}

/// <summary>
/// 基于MediatR的通用命令处理器接口，处理类型为TCommand的命令请求，并返回类型为TResponse的响应
/// </summary>
/// <typeparam name="TCommand">命令请求</typeparam>
/// <typeparam name="TResponse">命令响应</typeparam>
public interface ICommandHandler<in TCommand, TResponse> 
    : IRequestHandler<TCommand, TResponse> 
    where TCommand : ICommand<TResponse> 
    where TResponse : notnull
{
}
