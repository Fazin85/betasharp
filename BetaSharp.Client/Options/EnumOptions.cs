namespace BetaSharp.Client.Options;

public class EnumOptions : java.lang.Object
{
    public static readonly EnumOptions MUSIC = new("options.music", true, false, 0);
    public static readonly EnumOptions SOUND = new("options.sound", true, false, 1);
    public static readonly EnumOptions INVERT_MOUSE = new("options.invertMouse", false, true, 2);
    public static readonly EnumOptions SENSITIVITY = new("options.sensitivity", true, false, 3);
    public static readonly EnumOptions RENDER_DISTANCE = new("options.renderDistance", false, false, 4);
    public static readonly EnumOptions VIEW_BOBBING = new("options.viewBobbing", false, true, 5);
    public static readonly EnumOptions FRAMERATE_LIMIT = new("options.framerateLimit", true, false, 8);
    public static readonly EnumOptions FOV = new("options.fov", true, false, 18);
    public static readonly EnumOptions DIFFICULTY = new("options.difficulty", false, false, 9);
    public static readonly EnumOptions GUI_SCALE = new("options.guiScale", false, false, 12);
    public static readonly EnumOptions ANISOTROPIC = new("Aniso Level", false, false, 13);
    public static readonly EnumOptions MIPMAPS = new("Mipmaps", false, true, 14);
    public static readonly EnumOptions DEBUG_MODE = new("Debug Mode", false, true, 15);
    public static readonly EnumOptions MSAA = new("MSAA", false, false, 16);
    public static readonly EnumOptions ENVIRONMENT_ANIMATION = new("Environment Anim", false, true, 17);

    private static readonly EnumOptions[] s_allValues =
    [
        MUSIC, SOUND, INVERT_MOUSE, SENSITIVITY, RENDER_DISTANCE, VIEW_BOBBING, FRAMERATE_LIMIT, FOV, DIFFICULTY, GUI_SCALE, ANISOTROPIC, MIPMAPS, DEBUG_MODE, MSAA, ENVIRONMENT_ANIMATION
    ];

    private readonly bool enumFloat;
    private readonly bool enumBoolean;
    private readonly string enumString;
    private readonly int ordinalValue;

    public static EnumOptions? getEnumOptions(int ordinal)
    {
        foreach (EnumOptions option in values())
        {
            if (option.returnEnumOrdinal() == ordinal)
            {
                return option;
            }
        }

        return null;
    }

    private EnumOptions(string var3, bool var4, bool var5, int ordinal)
    {
        enumString = var3;
        enumFloat = var4;
        enumBoolean = var5;
        ordinalValue = ordinal;
    }

    public bool getEnumFloat()
    {
        return enumFloat;
    }

    public bool getEnumBoolean()
    {
        return enumBoolean;
    }

    public int returnEnumOrdinal()
    {
        return ordinal();
    }

    public string getEnumString()
    {
        return enumString;
    }

    public int ordinal()
    {
        return ordinalValue;
    }

    public static EnumOptions[] values()
    {
        return s_allValues;
    }
}
