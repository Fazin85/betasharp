using java.io;
using java.util.regex;

namespace BetaSharp.Worlds.Storage;

public class DataFilenameFilter : java.lang.Object, FilenameFilter
{
    public static readonly Pattern PATTERN = Pattern.compile("c\\.(-?[0-9a-z]+)\\.(-?[0-9a-z]+)\\.dat");

    private DataFilenameFilter()
    {
    }

    public bool accept(java.io.File var1, string var2)
    {
        Matcher var3 = PATTERN.matcher(var2);
        return var3.matches();
    }
}