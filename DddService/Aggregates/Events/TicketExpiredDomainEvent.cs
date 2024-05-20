using DddService.Common;

namespace DddService.Aggregates.Events;

public record TicketExpiredDomainEvent
    (string Surename, string Name, string PassportNumber, DateTime DateOfTicketExpiry) : IDomainEvent;