using MediatR;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Order.Infrastructure.Persistence.Interceptors;

/// <summary>
/// 领域事件分发拦截器
/// <para>由应用层消费领域事件后，判断是否需要通过消息队列/集成事件总线发布</para>
/// </summary>
/// <param name="mediator"></param>
/// <param name="logger"></param>
public class DispatchDomainEventsInterceptor(IMediator mediator, ILogger<DispatchDomainEventsInterceptor> logger) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        try
        {
            DispatchDomainEventsAsync(eventData.Context)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }
        catch (Exception ex)
        {
            if (logger.IsEnabled(LogLevel.Error))
                logger.LogError(ex, "Synchronized distribution of domain domain events failed.");
        }
        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        await DispatchDomainEventsAsync(eventData.Context, cancellationToken);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private async Task DispatchDomainEventsAsync(DbContext? context, CancellationToken cancellationToken = default)
    {
        if (context is null) return;

        // 仅筛选包含领域事件的聚合根
        var aggregateRootEntries = context.ChangeTracker
            .Entries<IAggregateRoot>()
            .Where(entry => entry.Entity.DomainEvents.Any())
            .ToList();

        if (aggregateRootEntries.Count == 0) return;

        foreach (var aggregateRootEntry in aggregateRootEntries)
        {
            var aggregateRoot = aggregateRootEntry.Entity;
            var aggregateRootId = GetAggregateRootId(aggregateRoot);
            var typedAggregateRoot = aggregateRoot as AggregateRoot<object>;

            await ProcessDomainEventsAsync(typedAggregateRoot, aggregateRoot, aggregateRootId, cancellationToken);

            // 清空领域事件(确保事件仅分发一次，避免重复发布)
            aggregateRoot.ClearDomainEvents();
        }
    }

    private async Task ProcessDomainEventsAsync(
        AggregateRoot<object>? typedAggregateRoot,
        IAggregateRoot aggregateRoot,
        string aggregateRootId,
        CancellationToken cancellationToken = default)
    {
        // 收集带执行顺序的领域事件
        var domainEventRecords = typedAggregateRoot?.GetDomainEventRecords()
            .OrderBy(r => r.EventOrder)
            .Select(r => r.EventData)
            .Where(e => e is not null)
            ?? [];

        // 合并非泛型聚合根的领域事件
        var nonTypeddomainEvents = aggregateRoot.DomainEvents.Where(e => e is not null);

        // 过滤出有效的INotification类型事件
        var validdomainEvents = domainEventRecords
            .Concat(nonTypeddomainEvents)
            .OfType<INotification>()
            .ToList();

        if (validdomainEvents.Count == 0) return;

        // 发布领域事件
        foreach (var domainEvent in validdomainEvents)
        {
            var eventId = (domainEvent as IDomainEvent)?.EventId ?? Guid.Empty;
            var eventTypeName = domainEvent.GetType().FullName ?? "UnknownEvent";

            try
            {
                await mediator.Publish(domainEvent, cancellationToken);
                if (logger.IsEnabled(LogLevel.Information))
                    logger.LogInformation(
                    "Domain event {EventTypeName} (EventId: {EventId}) of aggregate root {AggregateRootId} published successfully.",
                    eventTypeName, eventId, aggregateRootId);
            }
            catch (Exception ex)
            {
                if (logger.IsEnabled(LogLevel.Error))
                    logger.LogError(
                    ex,
                    "Failed to publish domain event {EventTypeName} (EventId: {EventId}) for aggregate root {AggregateRootId}.",
                    eventTypeName, eventId, aggregateRootId);
            }
        }
    }

    private static string GetAggregateRootId(IAggregateRoot aggregateRoot)
    {
        return aggregateRoot switch
        {
            AggregateRoot<Guid> guidAgg => guidAgg.Id.ToString(),
            AggregateRoot<string> strAgg => strAgg.Id,
            AggregateRoot<long> longAgg => longAgg.Id.ToString(),
            AggregateRoot<int> intAgg => intAgg.Id.ToString(),
            _ => aggregateRoot.GetType().Name + "_UnknownId" + $"_{Guid.NewGuid()}"
        };
    }
}
