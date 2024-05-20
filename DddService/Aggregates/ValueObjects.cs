using System.Text.RegularExpressions;

namespace DddService.Aggregates;

public class UserId
{
    public Guid Value { get; }

    private UserId(Guid value)
    {
        Value = value;
    }

    public static UserId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new InvalidUserIdException(value);
        }

        return new UserId(value);
    }

    public static implicit operator Guid(UserId userId)
    {
        return userId.Value;
    }
}

public class PassportId
{
    public Guid Value { get; }

    private PassportId(Guid value)
    {
        Value = value;
    }

    public static PassportId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new InvalidPassportIdException(value);
        }

        return new PassportId(value);
    }

    public static implicit operator Guid(PassportId passportId)
    {
        return passportId.Value;
    }
}

public class PassportNumber
{
    public string Value { get; }

    private PassportNumber(string value)
    {
        Value = value;
    }

    public static PassportNumber Of(string value)
    {
        Regex regex = new Regex(@"^[0-9]+$");
        if (value.Length != 9 || !regex.IsMatch(value))
        {
            throw new InvalidPassportNumberException();
        }

        return new PassportNumber(value);
    }

    public static implicit operator string(PassportNumber passportNumber)
    {
        return passportNumber.Value;
    }
}
public class Surename
{
    public string Value { get; }

    private Surename(string value)
    {
        Value = value;
    }

    public static Surename Of(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidNameException();
        }

        return new Surename(value);
    }

    public static implicit operator string(Surename surename)
    {
        return surename.Value;
    }
}
public class Name
{
    public string Value { get; }

    private Name(string value)
    {
        Value = value;
    }

    public static Name Of(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidNameException();
        }

        return new Name(value);
    }

    public static implicit operator string(Name name)
    {
        return name.Value;
    }
}
public class DateOfBirth
{
    public DateTime Value { get; }

    private DateOfBirth(DateTime value)
    {
        Value = value;
    }

    public static DateOfBirth Of(DateTime value)
    {
        if (value > DateTime.Now)
        {
            throw new InvalidBirthDateException();
        }

        return new DateOfBirth(value);
    }

    public static implicit operator DateTime(DateOfBirth birthDate)
    {
        return birthDate.Value;
    }
}
public class DateOfExpiry
{
    public DateTime Value { get; }

    private DateOfExpiry(DateTime value)
    {
        Value = value;
    }

    public static DateOfExpiry Of(DateTime value)
    {
        if (value < DateTime.Now)
        {
            throw new InvalidDateOfPassportExpiryException();
        }

        return new DateOfExpiry(value);
    }

    public static implicit operator DateTime(DateOfExpiry expiryDate)
    {
        return expiryDate.Value;
    }
}

public class Ticket
{
    public DateTime DateOfTicketExpiry { get; }
    public double Price { get; }

    private Ticket(DateTime dateOfTicketExpiry, double price)
    {
        DateOfTicketExpiry = dateOfTicketExpiry;
        Price = price;
    }

    public static Ticket Of(DateTime dateOfExpiry, Passport passport)
    {
        double price = 0;
        if (dateOfExpiry < DateTime.Now)
        {
            throw new InvalidDateOfTicketExpiryException();
        }
        if (DateTimeExtensions.CalculateAge(passport.DateOfBirth) >= 60) price = 40;
        else price = 70;
        return new Ticket(dateOfExpiry, price);
    }

}
