using DddService.Aggregates.Events;
using MediatR;

namespace DddService.Handlers;

public class TicketExpiredDomainEventHandler: INotificationHandler<TicketExpiredDomainEvent>
{
    private readonly ILogger<TicketExpiredDomainEventHandler> _logger;
    
    public TicketExpiredDomainEventHandler(ILogger<TicketExpiredDomainEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(TicketExpiredDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogWarning($"Ticket has expired for the user {notification.Surename} {notification.Name} with PassportNumber={notification.PassportNumber}. DateOfTicketExpiry={notification.DateOfTicketExpiry}");
       
        return Task.CompletedTask;
    }
}