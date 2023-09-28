namespace API;

public static class DateTimeExtensions
{

    public static int CalculateAge(this DateOnly dob)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = today.Year - dob.Year;

        return _ = dob > today.AddYears(-age) ? age-- : age;
    }

}