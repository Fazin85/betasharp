using System.Reflection;

namespace BetaSharp.Client.Options;

[AttributeUsage(AttributeTargets.Field)]
public class OptionInfoAttribute : Attribute
{
    public string TranslationKey { get; }
    public bool IsFloat { get; }
    public bool IsBoolean { get; }

    public OptionInfoAttribute(string translationKey, bool isFloat = false, bool isBoolean = false)
    {
        TranslationKey = translationKey;
        IsFloat = isFloat;
        IsBoolean = isBoolean;
    }
}

public enum EnumOptions
{
    [OptionInfo("options.music", isFloat: true)]           Music,
    [OptionInfo("options.sound", isFloat: true)]           Sound,
    [OptionInfo("options.invertMouse", isBoolean: true)]   InvertMouse,
    [OptionInfo("options.sensitivity", isFloat: true)]     Sensitivity,
    [OptionInfo("options.renderDistance")]                  RenderDistance,
    [OptionInfo("options.viewBobbing", isBoolean: true)]   ViewBobbing,
    [OptionInfo("options.framerateLimit", isFloat: true)]   FramerateLimit,
    [OptionInfo("options.fov", isFloat: true)]              Fov,
    [OptionInfo("Brightness", isFloat: true)]               Brightness,
    [OptionInfo("VSync", isBoolean: true)]                  VSync,
    [OptionInfo("options.difficulty")]                      Difficulty,
    [OptionInfo("options.guiScale")]                        GuiScale,
    [OptionInfo("Aniso Level")]                             Anisotropic,
    [OptionInfo("Mipmaps", isBoolean: true)]                Mipmaps,
    [OptionInfo("Debug Mode", isBoolean: true)]             DebugMode,
    [OptionInfo("MSAA")]                                    Msaa,
    [OptionInfo("Environment Anim", isBoolean: true)]       EnvironmentAnimation,
}

public static class EnumOptionsExtensions
{
    private static readonly Dictionary<EnumOptions, OptionInfoAttribute> Cache = BuildCache();

    private static Dictionary<EnumOptions, OptionInfoAttribute> BuildCache()
    {
        var dict = new Dictionary<EnumOptions, OptionInfoAttribute>();
        foreach (var field in typeof(EnumOptions).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            if (field.GetCustomAttribute<OptionInfoAttribute>() is OptionInfoAttribute attr)
            {
                dict[(EnumOptions)field.GetValue(null)!] = attr;
            }
        }
        return dict;
    }

    public static OptionInfoAttribute GetInfo(this EnumOptions option) => Cache[option];
    public static bool IsFloat(this EnumOptions option) => Cache[option].IsFloat;
    public static bool IsBoolean(this EnumOptions option) => Cache[option].IsBoolean;
    public static string GetTranslationKey(this EnumOptions option) => Cache[option].TranslationKey;
}
