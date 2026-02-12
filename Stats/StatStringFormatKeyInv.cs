using betareborn.Client.Input;
using betareborn.Client.Resource.Language;
using betareborn.Stats.Achievements;

namespace betareborn.Stats
{

    public class StatStringFormatKeyInv : AchievementStatFormatter
    {
        readonly Minecraft theGame;
        private static readonly TranslationStorage localizedName = TranslationStorage.getInstance();


        public StatStringFormatKeyInv(Minecraft game)
        {
            theGame = game;
        }

        public String formatString(String key)
        {
            return localizedName.translateKeyFormat(key, Keyboard.getKeyName(theGame.options.keyBindInventory.keyCode));
        }
    }

}