namespace BuildingBlocks;

public static class SystemDateTime
{
    private static Func<DateTime> s_dateProvider = () => DateTime.UtcNow;

    public static void ForceDateTimeProvider(Func<DateTime> dateTimeProvider)
    {
        s_dateProvider = dateTimeProvider;
    }

    public static void Reset()
    {
        s_dateProvider = () => DateTime.UtcNow;
    }

    public static DateTime Now => s_dateProvider();
}