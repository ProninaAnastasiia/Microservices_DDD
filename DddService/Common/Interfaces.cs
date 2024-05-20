namespace DddService.Common;

// Aggregates Interfaces

public interface IAggregate
{
    void AddDomainEvent(IDomainEvent domainEvent);
    IReadOnlyList<IDomainEvent> GetDomainEvents();
    IEvent[] ClearDomainEvents();
}