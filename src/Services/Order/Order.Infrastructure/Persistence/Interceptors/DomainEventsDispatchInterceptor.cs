using MediatR;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Order.Infrastructure.Persistence.Interceptors;

public class DomainEventsDispatchInterceptor(IMediator mediator, ILogger<DomainEventsDispatchInterceptor> logger) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        try
        {
            DispatchAllDomainEventsAsync(eventData.Context)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }
        catch (Exception ex)
        {
            if (logger.IsEnabled(LogLevel.Error))
                logger.LogError(ex, "Synchronized distribution of domain events failed.");
        }
        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        await DispatchAllDomainEventsAsync(eventData.Context, cancellationToken);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private async Task DispatchAllDomainEventsAsync(DbContext? context, CancellationToken cancellationToken = default)
    {
        if (context is null) return;

        // 筛选所有已变更的聚合根
        var aggregateRootEntries = context.ChangeTracker
            .Entries<IAggregateRoot>()
            .Where(entry => entry.Entity.LocalEvents.Any() || entry.Entity.DistributedEvents.Any())
            .ToList();

        if (aggregateRootEntries.Count == 0) return;

        foreach (var aggregateRootEntry in aggregateRootEntries)
        {
            var aggregateRoot = aggregateRootEntry.Entity;
            var aggregateRootId = GetAggregateRootId(aggregateRoot);
            var typedAggregateRoot = aggregateRoot as AggregateRoot<object>;

            // 处理本地事件
            await ProcessLocalEventsAsync(typedAggregateRoot, aggregateRoot, aggregateRootId, cancellationToken);

            // 处理分布式事件
            await ProcessDistributedEventsAsync(typedAggregateRoot, aggregateRoot, aggregateRootId, cancellationToken);

            // 清空事件列表(确保事件只分发一次)
            aggregateRoot.ClearLocalEvents();
            aggregateRoot.ClearDistributedEvents();
        }
    }

    private async Task ProcessLocalEventsAsync(
        AggregateRoot<object>? typedAggregateRoot,
        IAggregateRoot aggregateRoot,
        string aggregateRootId,
        CancellationToken cancellationToken = default)
    {
        // 收集带顺序的本地事件
        var localEventRecords = typedAggregateRoot?.GetLocalEventRecords()
            .OrderBy(r => r.EventOrder)
            .Select(r => r.EventData)
            .Where(e => e is not null)
            ?? [];

        // 合并非泛型聚合根的本地事件
        var nonTypedLocalEvents = aggregateRoot.LocalEvents.Where(e => e is not null);

        // 转换为非空的INotification
        var validLocalEvents = localEventRecords
            .Concat(nonTypedLocalEvents)
            .OfType<INotification>()
            .ToList();

        if (validLocalEvents.Count == 0) return;

        // 发布本地事件
        foreach (var localEvent in validLocalEvents)
        {
            var eventId = (localEvent as IDomainEvent)?.EventId ?? Guid.Empty;
            try
            {
                await mediator.Publish<INotification>(localEvent, cancellationToken);
                if (logger.IsEnabled(LogLevel.Information))
                    logger.LogInformation("The local event {EventId} of the aggregated root {AggregateRootId} has been successfully published.",  eventId, aggregateRootId);
            }
            catch (Exception ex)
            {
                if (logger.IsEnabled(LogLevel.Error))
                    logger.LogError(ex, "Failed to publish local event {EventId} for aggregation root {AggregateRootId}", eventId, aggregateRootId);
            }
        }
    }

    private async Task ProcessDistributedEventsAsync(
        AggregateRoot<object>? typedAggregateRoot,
        IAggregateRoot aggregateRoot,
        string aggregateRootId,
        CancellationToken cancellationToken = default)
    {
        // 收集带顺序的分布式事件
        var distributedEventRecords = typedAggregateRoot?.GetDistributedEventRecords()
            .OrderBy(r => r.EventOrder)
            .Select(r => r.EventData)
            .Where(e => e is not null)
            ?? [];

        // 合并非泛型聚合根的分布式事件
        var nonTypedDistributedEvents = aggregateRoot.DistributedEvents.Where(e => e is not null);

        // 转换为非空的IDistributedDomainEvent
        var validDistributedEvents = distributedEventRecords
            .Concat(nonTypedDistributedEvents)
            .OfType<IDistributedDomainEvent>()
            .ToList();

        if (validDistributedEvents.Count == 0) return;

        // 发布分布式事件
        foreach (var distributedEvent in validDistributedEvents) 
        {
            var eventId = distributedEvent.EventId;
            try
            {
                // 此处替换为消息队列发布
                await mediator.Publish(distributedEvent, cancellationToken);
                if (logger.IsEnabled(LogLevel.Information))
                    logger.LogInformation("The distributed event {EventId} of the aggregated root {AggregateRootId} has been forwarded successfully", eventId, aggregateRootId);
            }
            catch (Exception ex)
            {
                if (logger.IsEnabled(LogLevel.Error))
                    logger.LogError(ex, "Distributed event {EventId} forwarding failed for aggregation root {AggregateRootId}", eventId, aggregateRootId);
                // 分布式事件失败抛出异常，确保数据一致性
                throw new InvalidOperationException($"Failed to publish distributed event {eventId} for aggregated root {aggregateRootId}", ex);
            }
        }
    }

    private static string GetAggregateRootId(IAggregateRoot aggregateRoot)
    {
        return aggregateRoot switch
        {
            AggregateRoot<Guid> guidAgg => guidAgg.Id.ToString(),
            AggregateRoot<string> strAgg => strAgg.Id,
            _ => aggregateRoot.GetType().Name + "_UnknownId"
        };
    }
}
