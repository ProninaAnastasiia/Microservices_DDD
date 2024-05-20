using DddService.Aggregates.Events;
using DddService.Common;

namespace DddService.Aggregates;

// Entity - Aggregates
public class User : Aggregate<UserId>
{
    public Passport Passport { get; private set; } = default!;
    public Ticket? Ticket { get; private set; }

    public static User Create(UserId id, Passport passport, Ticket? ticket = null, bool isDeleted = false)
    {
        var user = new User
        {
            Id = id,
            Passport = passport,
            Ticket = ticket
        };
        // События
        var @event = new UserCreatedDomainEvent(
            user.Id,
            user.Passport.Surename,
            user.Passport.Name,
            user.Passport.PassportNumber,
            user.Passport.DateOfBirth,
            user.Passport.DateOfExpiry,
            user.Ticket.DateOfTicketExpiry,
            user.Ticket.Price);

        user.AddDomainEvent(@event);
        return user;
    }
    
}

public class Passport : Entity<PassportId>
{
    public PassportNumber PassportNumber { get; private set; } = default!;
    public Surename Surename { get; private set; } = default!;
    public Name Name { get; private set; } = default!;
    public DateOfBirth DateOfBirth { get; private set; } = default!;
    public DateOfExpiry DateOfExpiry { get; private set; } = default!;

    public static Passport Create(PassportId id, PassportNumber number, Surename surename, Name name, DateOfBirth birth, DateOfExpiry expiry, bool isDeleted = false)
    {
        var passport = new Passport
        {
            Id = id,
            PassportNumber = number,
            Surename = surename,
            Name = name,
            DateOfBirth = birth,
            DateOfExpiry = expiry,
        };
        
        return passport;
    }
}