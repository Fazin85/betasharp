using java.lang;

namespace BetaSharp.Client.Options;

public class EnumOptionsMappingHelper : java.lang.Object
{
    public static readonly int[] enumOptionsMappingHelperArray = new int[256];

    static EnumOptionsMappingHelper()
    {
        try
        {
            enumOptionsMappingHelperArray[EnumOptions.INVERT_MOUSE.ordinal()] = 1;
        }
        catch (NoSuchFieldError) { }

        try
        {
            enumOptionsMappingHelperArray[EnumOptions.VIEW_BOBBING.ordinal()] = 2;
        }
        catch (NoSuchFieldError) { }

        try
        {
            enumOptionsMappingHelperArray[EnumOptions.MIPMAPS.ordinal()] = 3;
        }
        catch (NoSuchFieldError) { }

        try
        {
            enumOptionsMappingHelperArray[EnumOptions.DEBUG_MODE.ordinal()] = 4;
        }
        catch (NoSuchFieldError) { }

        try
        {
            enumOptionsMappingHelperArray[EnumOptions.ENVIRONMENT_ANIMATION.ordinal()] = 5;
        }
        catch (NoSuchFieldError) { }
    }
}
