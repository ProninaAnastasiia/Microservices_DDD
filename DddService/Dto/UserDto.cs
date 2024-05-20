using DddService.Aggregates;

namespace DddService.Dto;
public record UserDto(string Id, Passport Passport, Ticket Ticket);

public record UserInputModel(string PassportNumber, string Surename, string Name, DateTime DateOfBirth, DateTime DateOfPassportExpiry);