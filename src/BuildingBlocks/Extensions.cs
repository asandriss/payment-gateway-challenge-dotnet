namespace BuildingBlocks;

public static class Extensions
{
    public static int GetLastFourDigits(this long value) => (int)(value % 10_000);
}