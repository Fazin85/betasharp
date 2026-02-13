using java.lang;
using OperatingSystem = BetaSharp.Util.OperatingSystem;

namespace BetaSharp;

public class EnumOSMappingHelper
{
    public static readonly int[] enumOSMappingArray = new int[System.Enum.GetValues<OperatingSystem>().Length];

    static EnumOSMappingHelper()
    {
        try
        {
            enumOSMappingArray[(int)Util.OperatingSystem.linux] = 1;
        }
        catch (NoSuchFieldError var4)
        {
        }

        try
        {
            enumOSMappingArray[(int)Util.OperatingSystem.solaris] = 2;
        }
        catch (NoSuchFieldError var3)
        {
        }

        try
        {
            enumOSMappingArray[(int)Util.OperatingSystem.windows] = 3;
        }
        catch (NoSuchFieldError var2)
        {
        }

        try
        {
            enumOSMappingArray[(int)Util.OperatingSystem.macos] = 4;
        }
        catch (NoSuchFieldError var1)
        {
        }

    }
}