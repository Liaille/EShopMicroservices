using MediatR;

namespace Order.Domain.Abstractions;

public abstract class DomainEvent : IDomainEvent, INotification
{
    public Guid EventId { get; }

    public DateTime OccurredOn { get; }

    public string EventType { get; }

    protected DomainEvent()
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        EventType = GetType().FullName ?? string.Empty;
    }

    protected DomainEvent(Guid eventId, DateTime occurredOn)
    {
        EventId = eventId;
        OccurredOn = occurredOn;
        EventType = GetType().FullName ?? string.Empty;
    }
}
