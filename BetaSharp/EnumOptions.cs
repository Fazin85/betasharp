namespace BetaSharp;

public class EnumOptions : java.lang.Object
{
    public static readonly EnumOptions MUSIC = new EnumOptions("options.music", true, false, 0);
    public static readonly EnumOptions SOUND = new EnumOptions("options.sound", true, false, 1);
    public static readonly EnumOptions INVERT_MOUSE = new EnumOptions("options.invertMouse", false, true, 2);
    public static readonly EnumOptions SENSITIVITY = new EnumOptions("options.sensitivity", true, false, 3);
    public static readonly EnumOptions RENDER_DISTANCE = new EnumOptions("options.renderDistance", false, false, 4);
    public static readonly EnumOptions VIEW_BOBBING = new EnumOptions("options.viewBobbing", false, true, 5);
    public static readonly EnumOptions FRAMERATE_LIMIT = new EnumOptions("options.framerateLimit", true, false, 8);
    public static readonly EnumOptions FOV = new EnumOptions("options.fov", true, false, 18);
    public static readonly EnumOptions DIFFICULTY = new EnumOptions("options.difficulty", false, false, 9);
    public static readonly EnumOptions GUI_SCALE = new EnumOptions("options.guiScale", false, false, 12);
    public static readonly EnumOptions ANISOTROPIC = new EnumOptions("Aniso Level", false, false, 13);
    public static readonly EnumOptions MIPMAPS = new EnumOptions("Mipmaps", false, true, 14);
    public static readonly EnumOptions DEBUG_MODE = new EnumOptions("Debug Mode", false, true, 15);
    public static readonly EnumOptions MSAA = new EnumOptions("MSAA", false, false, 16);
    public static readonly EnumOptions ENVIRONMENT_ANIMATION = new EnumOptions("Environment Anim", false, true, 17);

    private static readonly EnumOptions[] allValues = new EnumOptions[]
    {
        MUSIC, SOUND, INVERT_MOUSE, SENSITIVITY, RENDER_DISTANCE, VIEW_BOBBING, FRAMERATE_LIMIT, FOV, DIFFICULTY, GUI_SCALE, ANISOTROPIC, MIPMAPS, DEBUG_MODE, MSAA, ENVIRONMENT_ANIMATION
    };

    private readonly bool enumFloat;
    private readonly bool enumBoolean;
    private readonly string enumString;
    private readonly int ordinalValue;

    public static EnumOptions getEnumOptions(int var0)
    {
        EnumOptions[] var1 = values();
        int var2 = var1.Length;

        for (int var3 = 0; var3 < var2; ++var3)
        {
            EnumOptions var4 = var1[var3];
            if (var4.returnEnumOrdinal() == var0)
            {
                return var4;
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
        return allValues;
    }
}