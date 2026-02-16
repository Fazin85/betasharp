using BetaSharp.Client.Input;

namespace BetaSharp.Client.Achievements;

public class StatStringFormatKeyInv(Minecraft game) : AchievementStatFormatter
{
    private readonly Minecraft _mc = game;
    private static readonly TranslationStorage s_localizedName = TranslationStorage.Instance;

    public string formatString(string key)
    {
        return s_localizedName.TranslateKeyFormat(key, Keyboard.getKeyName(_mc.options.keyBindInventory.keyCode));
    }
}
