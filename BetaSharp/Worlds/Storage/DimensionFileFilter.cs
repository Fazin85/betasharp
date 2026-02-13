using java.io;
using java.util.regex;

namespace BetaSharp.Worlds.Storage;

public class DimensionFileFilter : FileFilter
{

    public static readonly Pattern PATTERN = Pattern.compile("[0-9a-z]|([0-9a-z][0-9a-z])");

    private DimensionFileFilter()
    {
    }

    public bool accept(java.io.File var1)
    {
        if (var1.isDirectory())
        {
            Matcher var2 = PATTERN.matcher(var1.getName());
            return var2.matches();
        }
        else
        {
            return false;
        }
    }
}