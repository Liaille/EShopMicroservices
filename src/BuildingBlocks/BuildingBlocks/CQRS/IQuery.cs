using MediatR;

namespace BuildingBlocks.CQRS;

/// <summary>
/// 基于MediatR的通用查询接口，表示一个查询请求，返回类型为TResponse
/// </summary>
/// <typeparam name="TResponse"></typeparam>
public interface IQuery<out TResponse> : IRequest<TResponse> where TResponse : notnull
{
}
