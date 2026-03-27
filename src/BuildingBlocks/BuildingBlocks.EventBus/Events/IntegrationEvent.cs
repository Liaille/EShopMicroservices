namespace BuildingBlocks.EventBus.Events;

public record IntegrationEvent
{
    public Guid Id { get; init; }

    public DateTime OccurredOn { get; init; }

    public string EventType => GetType().FullName!;

    public IntegrationEvent()
    {
        Id = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
    }
}
