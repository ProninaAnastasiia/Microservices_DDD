using System.Text.Json;
using DddService.Aggregates.Events;
using DddService.Dto;
using DddService.EventBus;
using MediatR;
using Messages;
using Microsoft.EntityFrameworkCore;

namespace DddService.Features;

public record GetUserByPassport : IRequest<UserDto>
{
    public string PassportNumber { get; }

    public GetUserByPassport(string number)
    {
        PassportNumber = number;
    }
}

public class GetUserByPassportHandler : IRequestHandler<GetUserByPassport, UserDto>
{
    private readonly UserAggregateDbContext _db;
    private readonly IMediator _mediator;
    private readonly KafkaProducerService _kafkaProducerService;
    public GetUserByPassportHandler(UserAggregateDbContext db, KafkaProducerService kafkaProducerService, IMediator mediator)
    {
        _db = db;
        _kafkaProducerService = kafkaProducerService;
        _mediator = mediator;
    }

    public async Task<UserDto> Handle(GetUserByPassport request, CancellationToken cancellationToken)
    {
        var user = await _db.Users.AsNoTrackingWithIdentityResolution()
            .FirstOrDefaultAsync(u => u.Passport.PassportNumber.Value.Equals(request.PassportNumber));
        var passport = await _db.Passports.AsNoTrackingWithIdentityResolution()
            .FirstOrDefaultAsync(p => p.PassportNumber.Value.Equals(request.PassportNumber));
        
        if (user == null)
        {
            throw new Exception("Пользователь не найден.");
        }
        else if (user.Ticket.DateOfTicketExpiry < DateTime.Now)
        {
            var @event = new TicketExpiredDomainEvent(
                passport.Surename,
                passport.Name,
                passport.PassportNumber,
                user.Ticket.DateOfTicketExpiry);
            _mediator.Publish(@event);
        }
        var userEnteredMessage = new UserEnteredMessage(user.Id, passport.Surename, passport.Name,passport.PassportNumber, new DateTimeOffset(user.Ticket.DateOfTicketExpiry.ToUniversalTime()).ToUnixTimeSeconds().ToString() , new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString());
        
        await _kafkaProducerService.ProduceAsync("UsersEntered", JsonSerializer.Serialize(userEnteredMessage));

        return new UserDto(user.Id.Value.ToString(), passport, user.Ticket);
    }
}