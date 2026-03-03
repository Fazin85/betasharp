namespace BetaSharp.Client.Sound;

public static class DefaultMusicCategories
{
    public static readonly ResourceLocation Game = "game";
    public static readonly ResourceLocation Menu = "menu";

    public static void Register(SoundManager soundManager)
    {
        soundManager.RegisterMusicCategory(Game, minDelayTicks: 12000, maxDelayTicks: 24000);
        soundManager.RegisterMusicCategory(Menu, minDelayTicks: 20, maxDelayTicks: 600);
    }
}
