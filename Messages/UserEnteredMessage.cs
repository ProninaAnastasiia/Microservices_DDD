namespace Messages;

public record UserEnteredMessage(Guid Id, string Surename, string Name, string PassportNumber, string DateOfTicketExpiry, string timestamp);
