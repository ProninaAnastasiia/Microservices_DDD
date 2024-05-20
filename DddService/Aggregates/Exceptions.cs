using DddService.Common;

namespace DddService.Aggregates;

public class UserAlreadyExistException : ConflictException
{
    public UserAlreadyExistException(int? code = default) : base("User already exist!", code)
    {
    }
}
public class InvalidUserIdException : BadRequestException
{
    public InvalidUserIdException(Guid userId)
        : base($"userId: '{userId}' is invalid.")
    {
    }
}
public class InvalidPassportIdException : BadRequestException
{
    public InvalidPassportIdException(Guid passportId)
        : base($"passportId: '{passportId}' is invalid.")
    {
    }
}
public class InvalidPassportNumberException : BadRequestException
{
    public InvalidPassportNumberException() : base("Passport must contains 9 digits.")
    {
    }
}

public class InvalidNameException : BadRequestException
{
    public InvalidNameException() : base("Name and surename cannot be empty or whitespace.")
    {
    }
}

public class InvalidBirthDateException : BadRequestException
{
    public InvalidBirthDateException() : base("Date of births cannot be later than the present date.")
    {
    }
}

public class InvalidDateOfPassportExpiryException : BadRequestException
{
    public InvalidDateOfPassportExpiryException() : base("Your passport has expired! DateOfPassportExpiry cannot be earlier than the present date.")
    {
    }
}
public class InvalidDateOfTicketExpiryException : BadRequestException
{
    public InvalidDateOfTicketExpiryException() : base("DateOfTicketExpiry cannot be earlier than the present date.")
    {
    }
}