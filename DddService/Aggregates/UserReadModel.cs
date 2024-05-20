namespace DddService.Aggregates;

public class UserReadModel
{
    public required Guid Id { get; init; }
    public required Guid UserId { get; init; }
    public required string PassportNumber { get; init; }
    public required string Surename { get; init; }
    public required string Name { get; init; }
    public required DateTime DateOfBirth { get; init; }
    public required DateTime DateOfExpiry { get; init; }
    public Ticket? Ticket { get; init; }
    public required bool IsDeleted { get; init; }
}