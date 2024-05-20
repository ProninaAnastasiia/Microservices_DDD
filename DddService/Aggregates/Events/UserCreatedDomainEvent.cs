using DddService.Common;

namespace DddService.Aggregates.Events;

public record UserCreatedDomainEvent
    (Guid Id, string Surename, string Name, string PassportNumber, DateTime DateOfBirth, DateTime DateOfPassportExpiry,
        DateTime DateOfTicketExpiry, double TicketPrice) : IDomainEvent;