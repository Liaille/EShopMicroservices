using MediatR;

namespace BuildingBlocks.CQRS;

/// <summary>
/// 基于MediatR的通用查询处理器接口，处理类型为TQuery的查询请求，返回类型为TResponse
/// </summary>
/// <typeparam name="TQuery">查询请求</typeparam>
/// <typeparam name="TResponse">查询响应</typeparam>
public interface IQueryHandler<in TQuery, TResponse>
    : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
    where TResponse : notnull
{
}
