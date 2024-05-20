using DddService.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DddService.Features;

public record GetAllUsersQuery : IRequest<IList<UserDto>>;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IList<UserDto>>
{
    private readonly UserAggregateDbContext _db;

    public GetAllUsersQueryHandler(UserAggregateDbContext db)
    {
        _db = db;
    }

    public async Task<IList<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        return await _db.Users.AsNoTrackingWithIdentityResolution().Select(a => new UserDto(a.Id.Value.ToString(), a.Passport, a.Ticket)).ToListAsync();
    }
}
