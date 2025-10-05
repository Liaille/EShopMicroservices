using MediatR;

namespace BuildingBlocks.CQRS;

/// <summary>
/// 基于MediatR的通用命令接口，表示一个没有返回值的命令请求
/// <para>Unit由MediatR提供的一个表示没有返回值的类型</para>
/// </summary>
public interface ICommand : ICommand<Unit>
{
}

/// <summary>
/// 基于MediatR的通用命令接口，表示一个有返回值的命令请求，返回类型为TResponse
/// </summary>
/// <typeparam name="TResponse"></typeparam>
public interface ICommand<out TResponse> : IRequest<TResponse>
{
}
