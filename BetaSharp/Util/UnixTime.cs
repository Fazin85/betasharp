namespace BetaSharp.Util;

public static class UnixTime
{
    // i feel very stupid
    public static long ToMillis(this DateTimeOffset dateTime)
    {
        return dateTime.ToUnixTimeMilliseconds();
    }
}
