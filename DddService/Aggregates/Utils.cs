public static class DateTimeExtensions
{
    public static int CalculateAge(DateTime birthDate)
    {
        DateTime currentDate = DateTime.Today;

        int age = currentDate.Year - birthDate.Year;
        if (birthDate.AddYears(age) > currentDate)
        {
            age--;
        }
        return age;
    }
}