using DddService.Aggregates.Events;
using DddService.EventBus;
using MediatR;

namespace DddService.Handlers;

public class UserCreatedDomainEventHandler: INotificationHandler<UserCreatedDomainEvent>
{
    private readonly ILogger<UserCreatedDomainEventHandler> _logger;
    private readonly KafkaProducerService _kafkaProducerService;
    
    public UserCreatedDomainEventHandler(KafkaProducerService kafkaProducerService, ILogger<UserCreatedDomainEventHandler> logger)
    {
        _kafkaProducerService = kafkaProducerService;
        _logger = logger;
    }

    public Task Handle(UserCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"New user created {DateTime.Now}: ID={notification.Id}, Surename={notification.Surename}, " +
                               $"Name={notification.Name}, PassportNumber={notification.PassportNumber}, DateOfBirth={notification.DateOfBirth}," +
                               $"DateOfPassportExpiry={notification.DateOfPassportExpiry}, DateOfTicketExpiry={notification.DateOfTicketExpiry}, TicketPrice={notification.TicketPrice}");
       
        return Task.CompletedTask;
    }
}