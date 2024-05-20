using DddService.Aggregates;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DddService.Features;

public record CreateUserCommand(string Surename, string Name, string PassportNumber, DateTime DateOfBirth, 
    DateTime DateOfPassportExpiry) : IRequest<CreateUserResult>
{
    public Guid Id { get; init; } = Guid.NewGuid();
}

public record CreateUserResult(Guid Id);

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreateUserResult>
{
    private readonly UserAggregateDbContext _db;

    public CreateUserCommandHandler(UserAggregateDbContext db)
    {
        _db = db;
    }
    
    public async Task<CreateUserResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var users = await _db.Users.Include(u => u.Passport).AsNoTracking().ToListAsync();
        var existingUser = users.FirstOrDefault(u => u.Passport.PassportNumber.Value == request.PassportNumber);
        if (existingUser is not null)
        {
            throw new UserAlreadyExistException();
        }
        var passport = _db.Passports.Add(Passport.Create(PassportId.Of(Guid.NewGuid()), PassportNumber.Of(request.PassportNumber), 
            Surename.Of(request.Surename), Name.Of(request.Name), DateOfBirth.Of(request.DateOfBirth), DateOfExpiry.Of(request.DateOfPassportExpiry))).Entity;
        var ticket = Ticket.Of(DateTime.UtcNow.Date.AddDays(3), passport);
        var userEntity = _db.Users.Add(User.Create(UserId.Of(Guid.NewGuid()), passport, ticket)).Entity;
        await _db.SaveChangesAsync();
        
        return new CreateUserResult(userEntity.Id.Value);
    }
}